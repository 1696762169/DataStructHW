//#define DEBUG_INIT
//#define DEBUG_LINE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisplayBoard : MonoBehaviour
{
    [Tooltip("���ڵ����λ��")]
    public Vector3 rootPosition;
    [Tooltip("ÿ��߶�")]
    public int layerHeight;
    [Tooltip("�ڵ������")]
    public int gap;

    private UISprite bkSprite;
    private GameObject nodePrefab;
    private Vector3 heightOffset;
    private GameObject linePrefab;
    protected void Start()
    {
        Tools.LogNull<UISprite>(gameObject, "DisplayBoradδ�ҵ�չʾ����ͼƬ���", true);
        bkSprite = GetComponent<UISprite>();

        UIScrollView sv = gameObject.GetComponentInParent<UIScrollView>();
        Tools.LogNull(sv, "DisplayBorad��������ScrollView", true);

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
    /// �������ڵ�
    /// </summary>
    /// <returns>ʵ��Э�̵ĵ�����</returns>
    private IEnumerator ShowBTree()
    {
        // ��ʼ���ڵ������Ϣ
        NodeControl.Init();
        int mw = NodeControl.MidWidth;
        int bw = NodeControl.BorderWidth;
        // ��ȡ����Ľڵ�����洢ֵ�� ����ÿ���ܿ�Ȼ�����ʼ��
        List<int> nodeCount = new List<int>(), valueCount = new List<int>();
        GetLayerCount(nodeCount, valueCount);
        int layerWidth = (bw + gap) * nodeCount[nodeCount.Count - 1] + mw * valueCount[valueCount.Count - 1];
        List<int> layerStart = new List<int>();
        for (int i = 0; i < nodeCount.Count; ++i)
            layerStart.Add((layerWidth / nodeCount[i] - layerWidth) / 2);
        layerStart[layerStart.Count - 1] -= NodeControl.MidWidth * nodeCount.Count / 3;

        // ׼������ͼ�λ���ʾ
        const int countPerFrame = 500;
        int count = 0;
        int layer = 0;
        List<int> layerCur = new List<int>();
        for(int i = 0; i < nodeCount.Count; ++i)
            layerCur.Add(0);

        // ���зֶ�ͼ�λ���ʾ
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
                // ������ǰ���ɵĽڵ���
                if (++count % countPerFrame == 0)
                    yield return null;
                // �����½ڵ�
                GameObjectNode gn = queue.Dequeue();
                int x = Mathf.Max(layerStart[layer] + layerCount * (layerWidth / nodeCount[layer]),
                    lastX + gap + (gn.node.values.Count * NodeControl.MidWidth + NodeControl.BorderWidth) / 2);
                int y = (int)rootPosition.y - layer * layerHeight;
                GameObject obj = InstantiateNode(gn, new Vector3(x, y, 0));
                lastX = x + obj.GetComponent<UISprite>().width / 2;     // ��¼�������ɵĽڵ�λ��
                foreach (IntWithObject value in gn.node.values)         // Ϊ��ֵ�󶨶���
                    value.obj = obj;
                ++layerCount;

                // ����ӽڵ�
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

        // ���¿��϶������С
        bkSprite.width = layerWidth + NodeControl.MidWidth * layer;
        bkSprite.height = layerHeight * nodeCount.Count - (int)rootPosition.y;
        bkSprite.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// ͳ��ÿ��ڵ���
    /// </summary>
    /// <returns>����Ľڵ���</returns>
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
    /// �ڳ�����ʵ����һ���ڵ�
    /// </summary>
    /// <param name="node">�ڵ���Ϣ</param>
    /// <param name="localPos">�ڵ����λ��</param>
    private GameObject InstantiateNode(GameObjectNode node, Vector3 localPos)
    {
        // ���ɽڵ�
        GameObject nodeObj = Instantiate(nodePrefab, transform);
        nodeObj.transform.localPosition = localPos;
        nodeObj.GetComponent<NodeControl>().CreateObject(node.node);

        // ����������
        if (node.parent != null)
        {
            GameObject lineObj = Instantiate(linePrefab, transform);
            Vector3 widthOffset = Vector3.right * (NodeControl.MidWidth * node.childNum - NodeControl.MidWidth * node.node.parent.values.Count / 2);
            Vector3 selfPos = nodeObj.transform.localPosition + heightOffset - 2 * Vector3.up;
            Vector3 parentPos = node.parent.transform.localPosition - heightOffset + widthOffset;
            // ȷ��λ��
            lineObj.transform.localPosition = selfPos;
            // ȷ������
            float length = Vector3.Distance(selfPos, parentPos);
            lineObj.GetComponent<UISprite>().width = (int)length;
            // ������ת�Ƕ�
            float diff = parentPos.x - selfPos.x;
            float angle = Mathf.Acos(diff / length) * Mathf.Rad2Deg;
            // ȷ���Ƕ�
            lineObj.transform.eulerAngles = Vector3.forward * angle;
#if DEBUG_LINE
            print($"�ӽڵ�λ�ã�{nodeObj.transform.localPosition} ���ڵ�λ�ã�{node.parent.transform.localPosition}");
            print($"���ȣ�{length} sin��{layerHeight / length} �Ƕȣ�{angle}");
            print($"�ӽڵ�����{node.node.parent.children.Count} �ӽڵ���ţ�{node.childNum} ƫ������{node.childNum - (float)node.node.values.Count / 2}");
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
