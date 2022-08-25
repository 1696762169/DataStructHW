using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonWithInput : MonoBehaviour
{
    public UIInput input;

    /// <summary>
    /// 添加节点
    /// </summary>
    public void Add()
    {
        if (input.value != "")
        {
            if (UnityBTree.Instance.Add(new IntWithObject(int.Parse(Summit()))))
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
            if (UnityBTree.Instance.Remove(new IntWithObject(int.Parse(Summit()))))
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
            IntWithObject result = UnityBTree.Instance.Find(new IntWithObject(int.Parse(Summit())));
            if (result != null)
            {

            }
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
            UnityBTree.Instance.Add(new IntWithObject(Random.Range(BTreeMgr.Instance.addMin, BTreeMgr.Instance.addMax + 1)));
        UnityBTree.Instance.Show();
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    public void RemoveRangeMin()
    {
        if (input.value == null)
            BTreeMgr.Instance.removeMin = BTreeMgr.Instance.addMin;
        else
            BTreeMgr.Instance.removeMin = int.Parse(Summit());
    }
    public void RemoveRangeMax()
    {
        if (input.value == null)
            BTreeMgr.Instance.removeMax = BTreeMgr.Instance.addMax;
        else
            BTreeMgr.Instance.removeMax = int.Parse(Summit());
    }
    public void RemoveRange()
    {
        IntWithObject temp = new IntWithObject(0);
        for (int i = BTreeMgr.Instance.removeMin; i <= BTreeMgr.Instance.removeMax; ++i)
        {
            temp.value = i;
            UnityBTree.Instance.Remove(temp);
        }
        UnityBTree.Instance.Show();
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
