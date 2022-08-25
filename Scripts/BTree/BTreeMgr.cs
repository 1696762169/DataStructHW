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

    // ����·����ʾ��
    public UILabel pathLabel;

    // ����������������
    public UISprite nodeAdjust;
    public UISprite otherFuncs;

    // �޽ڵ���ʾ
    public GameObject noNode;

    /// <summary>
    /// ��ʾ����·��
    /// </summary>
    public void ShowFindingPath(bool clear)
    {
        // �������
        if (clear)
        {
            pathLabel.text = "";
            return;
        }

        List<int> path = UnityBTree.Instance.findingPath;
        string text = "����·�������ڵ�";
        // ���ҳɹ�
        if (path.Count == 0 || path[path.Count - 1] != -1)
        {
            foreach (int num in path)
                text += "->�ӽڵ�" + num;
            text += "\n�����ҳɹ���";
        }
        // ����ʧ��
        else
        {
            for (int i = 0; i < path.Count - 1; ++i)
                text += "->�ӽڵ�" + path[i];
            text += "\n������ʧ�ܣ�";
        }
        pathLabel.text = text;
    }

    /// <summary>
    /// �������й��ܰ�ťê��
    /// </summary>
    private void SetButtonAnchors()
    {
        // ��ȡ��������Ӷ���
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

        // ��ȡ�������ؼ��߶�
        int titleHeight = nodeAdjust.transform.Find("Title").GetComponent<UILabel>().height;
        titleHeight = titleHeight * 3 / 2;
        int nh = (nodeAdjust.height - titleHeight) / (ns.Count + 2);    // Ԥ��·����ʾռ����
        int oh = (otherFuncs.height - titleHeight) / (os.Count - 1);    // �����������ռһ��

        // ���ýڵ�༭���
        int gap = nh / 3;   // �����ؼ����Ԥ��Ϊ����߶�����֮һ
        int width = nh * 3; // ��ť���Ԥ��Ϊ�߶ȵ�����
        for (int i = 0; i < ns.Count; ++i)
        {
            ns[i].topAnchor.absolute = -titleHeight - i * nh;
            ns[i].bottomAnchor.absolute = ns[i].topAnchor.absolute - nh + gap;
            ns[i].leftAnchor.absolute = -width;
            ns[i].rightAnchor.absolute = 0;
        }

        // ���������������
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
        Tools.LogNull(displayPanel, "δ����ʾ���DisplayPanel��DisplayMgr", true);
        Tools.LogNull(displayContent, "δ����ʾ����DisplayContent��DisplayMgr", true);
        Tools.LogNull(pathLabel, "δ�󶨲���·����ʾ��pathLabel��DisplayMgr", true);
        Tools.LogNull(nodeAdjust, "δ�󶨽ڵ�༭������nodeAdjust��DisplayMgr", true);
        Tools.LogNull(otherFuncs, "δ������������otherFuncs��DisplayMgr", true);
        Tools.LogNull<UILabel>(noNode, "δ���޽ڵ���ʾnoNode��DisplayMgr", true);

        // ������ʾ����������
        Vector4 range = displayPanel.GetComponent<UIPanel>().baseClipRegion;
        displayPanel.transform.localPosition += new Vector3(range.x, range.y + range.w / 2, 0);

        // �����ؼ���С��λ��
        SetButtonAnchors();
    }
    protected void Awake()
    {
        instance = this;
    }
}
