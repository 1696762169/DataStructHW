using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BTreeMgr : MonoBehaviour
{
    private static BTreeMgr instance;
    public static BTreeMgr Instance => instance;

    public GameObject displayPanel;
    public GameObject displayContent;

    // ������ӷ�Χ
    public int addMin = -99999;
    public int addMax = 99999;
    // ����ɾ����Χ
    public int removeMin;
    public int removeMax;

    protected void Start()
    {
        Tools.LogNull(displayPanel, "δ����ʾ���DisplayPanel��DisplayMgr", true);
        Tools.LogNull(displayContent, "δ����ʾ����DisplayContent��DisplayMgr", true);
        Vector4 range = displayPanel.GetComponent<UIPanel>().baseClipRegion;
        displayPanel.transform.localPosition += new Vector3(range.x, range.y + range.w / 2, 0);
    }
    protected void Awake()
    {
        instance = this;
    }
}
