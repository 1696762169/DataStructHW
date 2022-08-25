//#define DEBUG_DETAIL
//#define DEBUG_TOTAL
//#define DEBUG_PARENT
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
public class BTree<T> where T : IComparable<T>
{
    public int Rank { get; }
    private readonly int minBranch;
    protected BTreeNode<T> root;
    protected Comparison<T> comparison;

    public BTree(int rank) : this(rank, (a, b) => a.CompareTo(b)) { }

    public BTree(int rank, Comparison<T> comp)
    {
        if (rank < 2)
            throw new ArgumentOutOfRangeException(nameof(rank), "B树的阶过小");
        Rank = rank;
        minBranch = (Rank + 1) / 2;
        root = new BTreeNode<T>();
        root.children.Add(null);
        comparison = comp;
    }

    public virtual bool Add(T value)
    {
        // 判断是否已存在节点
        BTreeNode<T> cur = root;
        Stack<int> insertLocas = new Stack<int>();
        int index;
        while (true)
        {
            index = 0;
            while (index < cur.values.Count)
            {
                int ret = comparison(value, cur.values[index]);
                if (ret > 0)
                    ++index;
                else if (ret == 0)
                    return false;           // 查找到节点存在 插入失败
                else
                {
                    insertLocas.Push(index);
                    break;
                }
            }
            if (index == cur.values.Count)
                insertLocas.Push(index);
            if (cur.children[index] == null)
                break;
            cur = cur.children[index] ?? cur;
        }

        // 添加节点
        index = insertLocas.Pop();
        cur.values.Insert(index, value);
        cur.children.Add(null);
            
        while (true)
        {
            // 直接添加
            if (cur.children.Count <= Rank)
                return true;
            // 增加新层级
            if (cur.parent == null)
            {
                cur.parent = new BTreeNode<T>();
                cur.parent.children.Add(null);
                root = cur.parent;
            }

            // 进行分裂
            if (insertLocas.Count == 0)
                index = 0;
            else
                index = insertLocas.Pop();
            cur.parent.values.Insert(index, cur.values[Rank / 2]);
            cur.parent.children.Insert(index, null);
            CreateChild(cur, index, true);
            CreateChild(cur, index + 1, false);
            cur = cur.parent;
        }
    }
    private void CreateChild(BTreeNode<T> cur, int index, bool left)
    {
        int low = left ? 0 : Rank / 2 + 1;
        int high = left ? Rank / 2 : Rank;
#pragma warning disable CS8602 // 解引用可能出现空引用。
        cur.parent.children[index] = new BTreeNode<T>();
        BTreeNode<T>? node = cur.parent.children[index];
        node.parent = cur.parent;
        for (int i = low; i < high; i++)
        {
            node.values.Add(cur.values[i]);
            node.children.Add(cur.children[i]);
            if (cur.children[i] != null)
                cur.children[i].parent = node;
        }
        node.children.Add(cur.children[high]);
        if (cur.children[high] != null)
            cur.children[high].parent = node;
#pragma warning restore CS8602 // 解引用可能出现空引用。
    }

    public virtual T Find(T value)
    {
        BTreeNode<T>? cur = root;
        while (cur != null)
        {
            int index = 0;
            while (index < cur.values.Count)
            {
                int ret = comparison(value, cur.values[index]);
                if (ret < 0)
                    break;
                else if (ret > 0)
                    ++index;
                else if (ret == 0)
                    return cur.values[index];
            }
            cur = cur.children[index];
        }
#pragma warning disable CS8603 // 可能返回 null 引用。
        return default;
#pragma warning restore CS8603 // 可能返回 null 引用。
    }

    public virtual bool Remove(T value)
    {
        // 判断是否存在待删除节点
        BTreeNode<T>? cur = root;
        Stack<int> removeLocas = new Stack<int>();
        int index;
        while (cur != null)
        {
            index = 0;
            bool found = false;
            while (index < cur.values.Count)
            {
                int ret = comparison(value, cur.values[index]);
                if (ret < 0)
                    break;
                else if (ret > 0)
                    ++index;
                else
                {
                    found = true;
                    break;
                }
            }
            removeLocas.Push(index);
            if (found)
                break;
            cur = cur.children[index];
        }
        if (cur == null)        //  未查找到则删除失败
            return false;
#if DEBUG_DETAIL || DEBUG_TOTAL
        Console.WriteLine($"---------------删除值{value}---------------");
#endif

        // 将非叶子节点交换到叶子节点上 并进行删除
        if (cur.children[0] != null)
        {
            BTreeNode<T> node = cur;
#if DEBUG_DETAIL
            Console.WriteLine("进行节点值交换，当前子树为：");
            Show(node, 0);
#endif
            cur = cur.children[removeLocas.Peek() + 1];
            index = removeLocas.Pop();
            removeLocas.Push(index + 1);
#pragma warning disable CS8602 // 解引用可能出现空引用。
            while (cur.children[0] != null)
            {
                cur = cur.children[0];
                removeLocas.Push(0);
            }
            removeLocas.Push(0);
            node.values[index] = cur.values[0];
#if DEBUG_DETAIL
            Console.WriteLine("交换完毕后，当前子树为：");
            Show(node, 0);
#endif
        }
        cur.values.RemoveAt(removeLocas.Pop());
        cur.children.RemoveAt(0);
#if DEBUG_DETAIL
        if (cur.children.Count >= minBranch)
        {
            Console.WriteLine($"直接删除，其它节点值为");
            foreach (T v in cur.values)
                Console.Write(v + " ");
            Console.WriteLine();
        }
#endif
        // 进行删除
        while (cur.children.Count < minBranch && cur != root)
        {
            index = removeLocas.Pop();
            // 尝试借兄弟节点的值
            if (BorrowValue(cur, index, true) || BorrowValue(cur, index, false))
                break;

            // 进行合并
            if (index == cur.parent.children.Count - 1)
                --index;
            MergeChildren(ref cur, index);
            // 判断是否需要更新根节点
            if (cur.parent == root)
            {
                if (cur.parent.children.Count == 1)
                {
                    root = cur;
                    root.parent = null;
                }
                break;
            }
            cur = cur.parent;
        }
#if DEBUG_TOTAL
        Console.WriteLine("删除完毕，当前B树整体如下：");
        Show();
#endif
#pragma warning restore CS8602 // 解引用可能出现空引用。
        return true;
    }
    private bool BorrowValue(BTreeNode<T>? cur, int index, bool left)
    {
        // 判定是否能够借值
        if (cur == null || cur.parent == null)
            return false;
        BTreeNode<T> parent = cur.parent;
        if (index == (left ? 0 : parent.children.Count - 1))
            return false;
        BTreeNode<T>? sib = parent.children[left ? index - 1 : index + 1];
        if (sib == null || sib.children.Count <= minBranch)
            return false;

        // 进行借值
#if DEBUG_DETAIL
        Console.WriteLine($"向{(left ? "左" : "右")}侧借值，当前父节点子树如下：");
        Show(cur.parent, 0);
#endif
        int pvi = left ? index - 1 : index;
        int svi = left ? sib.values.Count - 1 : 0;
        int sci = left ? sib.children.Count - 1 : 0;
        cur.values.Insert(left ? 0 : cur.values.Count, parent.values[pvi]);
        parent.values[pvi] = sib.values[svi];
        sib.values.RemoveAt(svi);
        BTreeNode<T>? child = sib.children[sci];
        cur.children.Insert(left ? 0 : cur.children.Count, child);
        if (child != null)
            child.parent = cur;
        sib.children.RemoveAt(sci);
#if DEBUG_DETAIL
        Console.WriteLine("借值完毕后，当前父节点子树如下：");
        Show(cur.parent, 0);
#endif
        return true;
    }
    private void MergeChildren(ref BTreeNode<T>? cur, int index)
    {
        if (cur == null || cur.parent == null)
            return;
        BTreeNode<T> parent = cur.parent;
        BTreeNode<T>? right = parent.children[index + 1];
        cur = parent.children[index];
        if (cur == null || right == null)
            return;
#if DEBUG_DETAIL
        Console.WriteLine($"合并父节点的第{index}与第{index + 1}颗子树，当前父节点子树如下：");
        Show(parent, 0);
#endif
        // 合并父节点内容至当前节点
        cur.values.Add(parent.values[index]);
        parent.values.RemoveAt(index);
        parent.children.RemoveAt(index + 1);

        // 合并右兄弟内容到当前节点
        foreach (T value in right.values)
            cur.values.Add(value);
        foreach (BTreeNode<T>? child in right.children)
        {
            if (child != null)
                child.parent = cur;
            cur.children.Add(child);
        }
#if DEBUG_DETAIL
        Console.WriteLine("合并完毕后，当前父节点子树如下：");
        Show(parent, 0);
#endif
    }

    public virtual void Show() => Show(root, 0);
    protected virtual void Show(BTreeNode<T> cur, int depth)
    {
        const string space = "     ";
        if (cur.children[0] == null)
        {
            for (int i = 0; i < depth; i++)
                Console.Write(space);
            Console.Write("(");
            foreach (T value in cur.values)
                Console.Write(value + " ");
            if (cur.values.Count == 0)
                Console.Write("空");
            Console.Write("\b)");
            Console.WriteLine();
            return;
        }
#pragma warning disable CS8604 // 引用类型参数可能为 null。
        for (int i = 0; i < cur.values.Count; i++)
        {
            Show(cur.children[i], depth + 1);
            for (int j = 0; j < depth; j++)
                Console.Write(space);
            Console.WriteLine(cur.values[i]);
        }
        if (cur.values.Count == 0)
        {
            for (int j = 0; j < depth; j++)
                Console.Write(space);
            Console.WriteLine("空");
        }
        Show(cur.children.Last(), depth + 1);
#if DEBUG_PARENT
        foreach (BTreeNode<T>? child in cur.children)
            if (child.parent != cur)
            {
                Console.WriteLine("父子不匹配!");
                Console.Write("父节点值：");
                foreach (T value in cur.values)
                    Console.Write(value + "");
                Console.WriteLine();
                Console.Write("子节点值：");
                foreach (T value in child.values)
                    Console.Write(value + "");
                Console.WriteLine();
                Console.Write("人贩子节点值：");
                foreach (T value in child.parent.values)
                    Console.Write(value + "");
                Console.WriteLine();
            }
#endif
#pragma warning restore CS8604 // 引用类型参数可能为 null。
    }

    public void Clear()
    {
        root = new BTreeNode<T>();
        root.children.Add(null);
    }
    public class BTreeNode<TN>
    {
        public List<TN> values;
        public List<BTreeNode<TN>?> children;
        public BTreeNode<TN>? parent;
        private int id;
        public BTreeNode()
        {
            values = new List<TN>();
            children = new List<BTreeNode<TN>?>();
            id = new Random().Next();
        }
    }
}