using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityBTree : BTree<int>
{
    private static UnityBTree instance = new UnityBTree(3);
    public static UnityBTree Instance => instance;
    public BTreeNode<int> Root => root;

    // ������ʾ����
    public GameObject displayContent;
    // ����·������
    public List<int> findingPath;
    private UnityBTree(int rank) : base(rank) { }

    public override void Show()
    {
        if (displayContent != null)
            GameObject.Destroy(displayContent);
        if (root.values.Count == 0)
        {
            BTreeMgr.Instance.noNode.SetActive(true);
        }
        else
        {
            displayContent = GameObject.Instantiate(BTreeMgr.Instance.displayContent, BTreeMgr.Instance.displayPanel.transform);
            BTreeMgr.Instance.noNode.SetActive(false);
        }
    }

    public override int Find(int value)
    {
        findingPath = new List<int>();
        BTreeNode<int> cur = root;
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
            findingPath.Add(index);
            cur = cur.children[index];
        }
        // ����ʧ��ʱ�ڲ���·����β���-1
        findingPath.Add(-1);
        return 0;
    }

    /// <summary>
    /// ���ý���
    /// </summary>
    public void Init(int rank)
    {
        if (displayContent != null)
            GameObject.Destroy(displayContent);
        instance = new UnityBTree(rank);
    }
}