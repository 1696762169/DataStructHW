//#define DEBUG_INIT
//#define DEBUG_LINE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisplayBoard : MonoBehaviour
{
    [Tooltip("根节点绘制位置")]
    public Vector3 rootPosition;
    [Tooltip("每层高度")]
    public int layerHeight;
    [Tooltip("节点横向间隔")]
    public int gap;

    private UISprite bkSprite;
    private GameObject nodePrefab;
    private Vector3 heightOffset;
    private GameObject linePrefab;
    protected void Start()
    {
        Tools.LogNull<UISprite>(gameObject, "DisplayBorad未找到展示背景图片组件", true);
        bkSprite = GetComponent<UISprite>();

        UIScrollView sv = gameObject.GetComponentInParent<UIScrollView>();
        Tools.LogNull(sv, "DisplayBorad父对象不是ScrollView", true);

        nodePrefab = Resources.Load<GameObject>("BTree/Node");
        heightOffset = (nodePrefab.GetComponent<UISprite>().height / 2) * Vector3.up;
        linePrefab = Resources.Load<GameObject>("BTree/Line");
#if DEBUG_INIT
        for (int i = 0; i < 50; ++i)
            UnityBTree.Instance.Add(Random.Range(-99999, 99999));
        //GameObjectNode root = new GameObjectNode(UnityBTree.Instance.Root);
        //InstantiateNode(root, rootPosition);
#endif
        StartCoroutine(ShowBTree());
    }

    /// <summary>
    /// 绘制树节点
    /// </summary>
    /// <returns>实现协程的迭代器</returns>
    private IEnumerator ShowBTree()
    {
        // 初始化节点管理信息
        NodeControl.Init();
        int mw = NodeControl.MidWidth;
        int bw = NodeControl.BorderWidth;
        // 获取各层的节点数与存储值数 计算每层总宽度绘制起始点
        List<int> nodeCount = new List<int>(), valueCount = new List<int>();
        GetLayerCount(nodeCount, valueCount);
        int layerWidth = (bw + gap) * nodeCount[nodeCount.Count - 1] + mw * valueCount[valueCount.Count - 1];
        List<int> layerStart = new List<int>();
        for (int i = 0; i < nodeCount.Count; ++i)
            layerStart.Add((layerWidth / nodeCount[i] - layerWidth) / 2);
        layerStart[layerStart.Count - 1] -= NodeControl.MidWidth * nodeCount.Count / 3;

        // 准备进行图形化显示
        const int countPerFrame = 500;
        int count = 0;
        int layer = 0;
        List<int> layerCur = new List<int>();
        for(int i = 0; i < nodeCount.Count; ++i)
            layerCur.Add(0);

        // 进行分段图形化显示
        Queue<GameObjectNode> queue = new Queue<GameObjectNode>();
        GameObjectNode root = new GameObjectNode(UnityBTree.Instance.Root);
        queue.Enqueue(root);

        while (true)
        {
            Queue<GameObjectNode> temp = new Queue<GameObjectNode>();
            int layerCount = 0;
#if DEBUG
            if (layer >= layerStart.Count)
                break;
#endif
            int lastX = layerStart[layer];
            while (queue.Count > 0)
            {
                // 计数当前生成的节点数
                if (++count % countPerFrame == 0)
                    yield return null;
                // 生成新节点
                GameObjectNode gn = queue.Dequeue();
                int x = Mathf.Max(layerStart[layer] + layerCount * (layerWidth / nodeCount[layer]),
                    lastX + gap + (gn.node.values.Count * NodeControl.MidWidth + NodeControl.BorderWidth) / 2);
                int y = (int)rootPosition.y - layer * layerHeight;
                GameObject obj = InstantiateNode(gn, new Vector3(x, y, 0));
                lastX = x + obj.GetComponent<UISprite>().width / 2;     // 记录本次生成的节点位置
                foreach (IntWithObject value in gn.node.values)         // 为数值绑定对象
                    value.obj = obj;
                ++layerCount;

                // 添加子节点
                if (gn.node.children[0] == null)
                    continue;
                for (int i = 0; i < gn.node.children.Count; ++i)
                {
                    GameObjectNode child = new GameObjectNode(gn.node.children[i], obj, i);
                    temp.Enqueue(child);
                }
            }
            if (temp.Count == 0)
                break;
            queue = temp;
            ++layer;
        }

        // 更新可拖动区域大小
        bkSprite.width = layerWidth + NodeControl.MidWidth * layer;
        bkSprite.height = layerHeight * nodeCount.Count - (int)rootPosition.y;
        bkSprite.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 统计每层节点数
    /// </summary>
    /// <returns>各层的节点数</returns>
    private void GetLayerCount(List<int> layerCountList, List<int> valueCountList)
    {
        BTree<IntWithObject>.BTreeNode<IntWithObject> cur = UnityBTree.Instance.Root;
        
        Queue<BTree<IntWithObject>.BTreeNode<IntWithObject>> queue = new Queue<BTree<IntWithObject>.BTreeNode<IntWithObject>>();
        queue.Enqueue(cur);
        while (true)
        {
            int layerCount = 0, valueCount = 0;
            Queue<BTree<IntWithObject>.BTreeNode<IntWithObject>> temp = new Queue<BTree<IntWithObject>.BTreeNode<IntWithObject>>();
            while (queue.Count > 0)
            {
                BTree<IntWithObject>.BTreeNode<IntWithObject> child = queue.Dequeue();
                ++layerCount;
                valueCount += child.values.Count;
                if (child.children[0] != null)
                    foreach (BTree<IntWithObject>.BTreeNode<IntWithObject> node in child.children)
                        temp.Enqueue(node);
            }
            layerCountList.Add(layerCount);
            valueCountList.Add(valueCount);
            if (temp.Count == 0)
                break;
            queue = temp;
        }
    }

    /// <summary>
    /// 在场景中实例化一个节点
    /// </summary>
    /// <param name="node">节点信息</param>
    /// <param name="localPos">节点相对位置</param>
    private GameObject InstantiateNode(GameObjectNode node, Vector3 localPos)
    {
        // 生成节点
        GameObject nodeObj = Instantiate(nodePrefab, transform);
        nodeObj.transform.localPosition = localPos;
        nodeObj.GetComponent<NodeControl>().CreateObject(node.node);

        // 生成连接线
        if (node.parent != null)
        {
            GameObject lineObj = Instantiate(linePrefab, transform);
            Vector3 widthOffset = Vector3.right * (NodeControl.MidWidth * node.childNum - NodeControl.MidWidth * node.node.parent.values.Count / 2);
            Vector3 selfPos = nodeObj.transform.localPosition + heightOffset - 2 * Vector3.up;
            Vector3 parentPos = node.parent.transform.localPosition - heightOffset + widthOffset;
            // 确定位置
            lineObj.transform.localPosition = selfPos;
            // 确定长度
            float length = Vector3.Distance(selfPos, parentPos);
            lineObj.GetComponent<UISprite>().width = (int)length;
            // 计算旋转角度
            float diff = parentPos.x - selfPos.x;
            float angle = Mathf.Acos(diff / length) * Mathf.Rad2Deg;
            // 确定角度
            lineObj.transform.eulerAngles = Vector3.forward * angle;
#if DEBUG_LINE
            print($"子节点位置：{nodeObj.transform.localPosition} 父节点位置：{node.parent.transform.localPosition}");
            print($"长度：{length} sin：{layerHeight / length} 角度：{angle}");
            print($"子节点数：{node.node.parent.children.Count} 子节点序号：{node.childNum} 偏移量：{node.childNum - (float)node.node.values.Count / 2}");
#endif
        }
        return nodeObj;
    }

    private class GameObjectNode
    {
        public GameObjectNode(BTree<IntWithObject>.BTreeNode<IntWithObject> node) : this(node, null, 0) { }
        public GameObjectNode(BTree<IntWithObject>.BTreeNode<IntWithObject> node, GameObject parent, int num)
        {
            this.node = node;
            this.parent = parent;
            childNum = num;
        }
        public BTree<IntWithObject>.BTreeNode<IntWithObject> node;
        public GameObject parent;
        public int childNum;
    }
}
