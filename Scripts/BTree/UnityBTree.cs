using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityBTree : BTree<IntWithObject>
{
    private static UnityBTree instance = new UnityBTree(3);
    public static UnityBTree Instance => instance;
    public BTreeNode<IntWithObject> Root => root;

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
public class IntWithObject : IComparable<IntWithObject>
{
    public IntWithObject(int value) => this.value = value;
    public int value;
    public GameObject obj;
    public int CompareTo(IntWithObject other) => value - other.value;
}
