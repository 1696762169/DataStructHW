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

        // 为除Find功能外的按钮/输入框绑定清除查找路径显示框的方法
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
    /// 添加节点
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
    /// 删除节点
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
    /// 查找节点并将移动至该处
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
    /// 设置批量处理范围下限
    /// </summary>
    public void SetRangeMin() => BTreeMgr.Instance.addMin = int.Parse(Summit(false));
    /// <summary>
    /// 设置批量处理范围上限
    /// </summary>
    public void SetRangeMax() => BTreeMgr.Instance.addMax = int.Parse(Summit(false));

    /// <summary>
    /// 批量添加
    /// </summary>
    public void AddMany()
    {
        int num = int.Parse(Summit(true));
        for (int i = 0; i < num; ++i)
            UnityBTree.Instance.Add(Random.Range(BTreeMgr.Instance.addMin, BTreeMgr.Instance.addMax + 1));
        UnityBTree.Instance.Show();
    }

    /// <summary>
    /// 批量删除
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
    /// 设置阶数
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
    /// 清除所有节点
    /// </summary>
    public void Clear()
    {
        UnityBTree.Instance.Clear();
        UnityBTree.Instance.Show();
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    public void Exit() => Application.Quit();

    /// <summary>
    /// 获取并清除绑定的输入框中的内容
    /// </summary>
    private string Summit(bool clear = true)
    {
        string value = input.value;
        if (clear)
            input.value = "";
        return value;
    }
}
