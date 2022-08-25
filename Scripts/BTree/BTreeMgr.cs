using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BTreeMgr : MonoBehaviour
{
    private static BTreeMgr instance;
    public static BTreeMgr Instance => instance;

    public GameObject displayPanel;
    public GameObject displayContent;

    // 批量添加范围
    public int addMin = -99999;
    public int addMax = 99999;
    // 批量删除范围
    public int removeMin;
    public int removeMax;

    protected void Start()
    {
        Tools.LogNull(displayPanel, "未绑定显示面板DisplayPanel至DisplayMgr", true);
        Tools.LogNull(displayContent, "未绑定显示内容DisplayContent至DisplayMgr", true);
        Vector4 range = displayPanel.GetComponent<UIPanel>().baseClipRegion;
        displayPanel.transform.localPosition += new Vector3(range.x, range.y + range.w / 2, 0);
    }
    protected void Awake()
    {
        instance = this;
    }
}
