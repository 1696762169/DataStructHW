using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;
    public static GameMgr Instance => instance;

    // 绑定面板
    public GameObject startPanel;
    public GameObject playPanel;
    public GameObject endPanel;

    // 参与人数
    public int totalNum;
    public const int maxNum = 99;
    public const int minNum = 2;

    protected void Start()
    {
        Tools.LogNull<UIPanel>(startPanel, "未绑定开始界面至RandomGameMgr", true);
        Tools.LogNull<UIPanel>(playPanel, "未绑定游玩界面至RandomGameMgr", true);
        Tools.LogNull<UIPanel>(endPanel, "未绑定结束界面至RandomGameMgr", true);
    }

    /// <summary>
    /// 通过输入框获取总人数 合法则开始游戏
    /// </summary>
    public void SetTotalNumAndStart(UIInput input)
    {
        int value = int.Parse(input.value);
        input.value = "";
        if (minNum <= value && value <= maxNum)
        {
            totalNum = value;
            HideStartPanel();
            ShowPlayPanel();
        }
    }

    // 面板开启与关闭函数
    public void ShowStartPanel() => startPanel.SetActive(true);
    public void ShowPlayPanel() => playPanel.SetActive(true);
    public void ShowEndPanel() => endPanel.SetActive(true);
    public void HideStartPanel() => startPanel.SetActive(false);
    public void HidePlayPanel() => playPanel.SetActive(false);
    public void HideEndPanel() => endPanel.SetActive(false);
    public void Exit() => Application.Quit();

    protected void Awake()
    {
        instance = this;
    }
}
