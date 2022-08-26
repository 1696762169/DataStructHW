using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;
    public static GameMgr Instance => instance;

    // �����
    public GameObject startPanel;
    public GameObject playPanel;
    public GameObject endPanel;

    // ��������
    public int totalNum;
    public const int maxNum = 99;
    public const int minNum = 2;

    protected void Start()
    {
        Tools.LogNull<UIPanel>(startPanel, "δ�󶨿�ʼ������RandomGameMgr", true);
        Tools.LogNull<UIPanel>(playPanel, "δ�����������RandomGameMgr", true);
        Tools.LogNull<UIPanel>(endPanel, "δ�󶨽���������RandomGameMgr", true);
    }

    /// <summary>
    /// ͨ��������ȡ������ �Ϸ���ʼ��Ϸ
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

    // ��忪����رպ���
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
