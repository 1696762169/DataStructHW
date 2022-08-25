using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonWithInput : MonoBehaviour
{
    public UIInput input;
    private UIButton button;

    protected void Start()
    {
        button = GetComponent<UIButton>();
        if (button == null)
            return;

        // Ϊ��Find������İ�ť/�������������·����ʾ��ķ���
        bool find = false;
        foreach (var ed in button.onClick)
        {
            if (ed.methodName == "Find")
            {
                find = true;
                break;
            }
        }
        if (!find)
        {
            EventDelegate clear = new EventDelegate(() => BTreeMgr.Instance.ShowFindingPath(true));
            button.onClick.Add(clear);
            if (input != null)
                input.onSubmit.Add(clear);
        }
    }

    /// <summary>
    /// ��ӽڵ�
    /// </summary>
    public void Add()
    {
        if (input.value != "")
        {
            if (UnityBTree.Instance.Add(int.Parse(Summit())))
                UnityBTree.Instance.Show();
        }
    }
    /// <summary>
    /// ɾ���ڵ�
    /// </summary>
    public void Remove()
    {
        if (input.value != "")
        {
            if (UnityBTree.Instance.Remove(int.Parse(Summit())))
                UnityBTree.Instance.Show();
        }
    }
    /// <summary>
    /// ���ҽڵ㲢���ƶ����ô�
    /// </summary>
    public void Find()
    {
        if (input.value != "")
        {
            UnityBTree.Instance.Find(int.Parse(Summit()));
            BTreeMgr.Instance.ShowFindingPath(false);
        }
    }

    /// <summary>
    /// ������������Χ����
    /// </summary>
    public void SetRangeMin() => BTreeMgr.Instance.addMin = int.Parse(Summit(false));
    /// <summary>
    /// ������������Χ����
    /// </summary>
    public void SetRangeMax() => BTreeMgr.Instance.addMax = int.Parse(Summit(false));

    /// <summary>
    /// �������
    /// </summary>
    public void AddMany()
    {
        int num = int.Parse(Summit(true));
        for (int i = 0; i < num; ++i)
            UnityBTree.Instance.Add(Random.Range(BTreeMgr.Instance.addMin, BTreeMgr.Instance.addMax + 1));
        UnityBTree.Instance.Show();
    }

    /// <summary>
    /// ����ɾ��
    /// </summary>
    public void RemoveRangeMin()
    {
        if (input.value == "")
            BTreeMgr.Instance.removeMin = BTreeMgr.Instance.addMin;
        else
            BTreeMgr.Instance.removeMin = int.Parse(Summit());
    }
    public void RemoveRangeMax()
    {
        if (input.value == "")
            BTreeMgr.Instance.removeMax = BTreeMgr.Instance.addMax;
        else
            BTreeMgr.Instance.removeMax = int.Parse(Summit());
    }
    public void RemoveRange()
    {
        for (int i = BTreeMgr.Instance.removeMin; i <= BTreeMgr.Instance.removeMax; ++i)
            UnityBTree.Instance.Remove(i);
        UnityBTree.Instance.Show();
    }

    /// <summary>
    /// ���ý���
    /// </summary>
    public void SetRank()
    {
        if (input.value != "")
        {
            int rank = int.Parse(Summit());
            if (rank >= 3)
            {
                UnityBTree.Instance.Init(rank);
                UnityBTree.Instance.Show();
            }
        }
    }

    /// <summary>
    /// ������нڵ�
    /// </summary>
    public void Clear()
    {
        UnityBTree.Instance.Clear();
        UnityBTree.Instance.Show();
    }

    /// <summary>
    /// �˳�����
    /// </summary>
    public void Exit() => Application.Quit();

    /// <summary>
    /// ��ȡ������󶨵�������е�����
    /// </summary>
    private string Summit(bool clear = true)
    {
        string value = input.value;
        if (clear)
            input.value = "";
        return value;
    }
}
