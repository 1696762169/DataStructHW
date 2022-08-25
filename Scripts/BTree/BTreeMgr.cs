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

    // 查找路径显示框
    public UILabel pathLabel;

    // 控制面板的两个区域
    public UISprite nodeAdjust;
    public UISprite otherFuncs;

    // 无节点提示
    public GameObject noNode;

    /// <summary>
    /// 显示查找路径
    /// </summary>
    public void ShowFindingPath(bool clear)
    {
        // 清除内容
        if (clear)
        {
            pathLabel.text = "";
            return;
        }

        List<int> path = UnityBTree.Instance.findingPath;
        string text = "查找路径：根节点";
        // 查找成功
        if (path.Count == 0 || path[path.Count - 1] != -1)
        {
            foreach (int num in path)
                text += "->子节点" + num;
            text += "\n（查找成功）";
        }
        // 查找失败
        else
        {
            for (int i = 0; i < path.Count - 1; ++i)
                text += "->子节点" + path[i];
            text += "\n（查找失败）";
        }
        pathLabel.text = text;
    }

    /// <summary>
    /// 设置所有功能按钮锚点
    /// </summary>
    private void SetButtonAnchors()
    {
        // 获取两个面板子对象
        List<UISprite> ns = new List<UISprite>();
        List<UISprite> os = new List<UISprite>();
        for (int i = 0; i < nodeAdjust.transform.childCount; ++i)
        {
            UISprite s = nodeAdjust.transform.GetChild(i).GetComponent<UISprite>();
            if (s != null)
                ns.Add(s);
        }
        for (int i = 0; i < otherFuncs.transform.childCount; ++i)
        {
            UISprite s = otherFuncs.transform.GetChild(i).GetComponent<UISprite>();
            if (s != null)
                os.Add(s);
        }

        // 获取两个面板控件高度
        int titleHeight = nodeAdjust.transform.Find("Title").GetComponent<UILabel>().height;
        titleHeight = titleHeight * 3 / 2;
        int nh = (nodeAdjust.height - titleHeight) / (ns.Count + 2);    // 预设路径显示占两行
        int oh = (otherFuncs.height - titleHeight) / (os.Count - 1);    // 最后两个功能占一行

        // 设置节点编辑面板
        int gap = nh / 3;   // 两个控件间隔预设为本体高度三分之一
        int width = nh * 3; // 按钮宽度预设为高度的三倍
        for (int i = 0; i < ns.Count; ++i)
        {
            ns[i].topAnchor.absolute = -titleHeight - i * nh;
            ns[i].bottomAnchor.absolute = ns[i].topAnchor.absolute - nh + gap;
            ns[i].leftAnchor.absolute = -width;
            ns[i].rightAnchor.absolute = 0;
        }

        // 设置其他功能面板
        gap = oh / 3;
        for (int i = 0; i < os.Count - 2; ++i)
        {
            os[i].topAnchor.absolute = -titleHeight - i * oh;
            os[i].bottomAnchor.absolute = os[i].topAnchor.absolute - oh + gap;
            os[i].leftAnchor.absolute = -width;
            os[i].rightAnchor.absolute = 0;
        }
        for (int i = os.Count - 2; i < os.Count; ++i)
        {
            os[i].topAnchor.absolute = -titleHeight - (os.Count - 2) * oh;
            os[i].bottomAnchor.absolute = os[i].topAnchor.absolute - oh + gap;
        }
    }

    protected void Start()
    {
        Tools.LogNull(displayPanel, "未绑定显示面板DisplayPanel至DisplayMgr", true);
        Tools.LogNull(displayContent, "未绑定显示内容DisplayContent至DisplayMgr", true);
        Tools.LogNull(pathLabel, "未绑定查找路径显示框pathLabel至DisplayMgr", true);
        Tools.LogNull(nodeAdjust, "未绑定节点编辑功能区nodeAdjust至DisplayMgr", true);
        Tools.LogNull(otherFuncs, "未绑定其他功能区otherFuncs至DisplayMgr", true);
        Tools.LogNull<UILabel>(noNode, "未绑定无节点提示noNode至DisplayMgr", true);

        // 修正显示区域至中央
        Vector4 range = displayPanel.GetComponent<UIPanel>().baseClipRegion;
        displayPanel.transform.localPosition += new Vector3(range.x, range.y + range.w / 2, 0);

        // 调整控件大小与位置
        SetButtonAnchors();
    }
    protected void Awake()
    {
        instance = this;
    }
}
