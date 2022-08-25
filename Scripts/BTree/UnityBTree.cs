using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityBTree : BTree<int>
{
    private static UnityBTree instance = new UnityBTree(3);
    public static UnityBTree Instance => instance;
    public BTreeNode<int> Root => root;

    public GameObject displayContent;
    private UnityBTree(int rank) : base(rank) { }

    public override void Show()
    {
        if (displayContent != null)
            GameObject.Destroy(displayContent);
        displayContent = GameObject.Instantiate(BTreeMgr.Instance.displayContent, BTreeMgr.Instance.displayPanel.transform);
    }

    public void Init(int rank) => instance = new UnityBTree(rank);
}