using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayingControl : MonoBehaviour
{
    [Tooltip("�����ʾ����")]
    public UIWidget playerArea;
    [Tooltip("��ǰ�����ɫ")]
    public Color curPlayerColor;
    // ��Ҷ���Ԥ����
    private GameObject playerPrefab;
    // ����б�
    private List<GameObject> players;
    // ��ǰ��ұ��(��0��ʼ)
    private int curPlayerNum;

    [Tooltip("�����ʾ����(�䵱����)")]
    public UILabel dice;
    // ���ӵ�ǰ������
    private int diceCurNum;
    // һ����Ϸִ�����
    private bool diceDone;

    [Tooltip("�Զ�ִ�а�ť��ǩ")]
    public UILabel autoLabel;
    // �Զ�ִ��
    private bool auto;

    [Tooltip("���ֺ�����ʾ����")]
    public UIWidget outNumberArea;
    // ���ֺ���Ԥ����
    private GameObject outNumberPrefab;
    private UIGrid grid;

    // ��ʼ����ʶ
    private bool inited;

    // ��Ϸ�������
    private GameObject endPanel => GameMgr.Instance.endPanel;
    // ��Ϸ������ʶ
    private bool GameOver => players.Count <= 1;

    /// <summary>
    /// ����һ����Ϸ
    /// </summary>
    public void Play()
    {
        if (diceDone)
            StartCoroutine(ThrowDice());
    }
    /// <summary>
    /// �Զ�������Ϸ
    /// </summary>
    public void AutoPlay()
    {
        if (!auto)
        {
            auto = true;
            autoLabel.text = "ֹͣ�Զ�";
            if (diceDone)
                StartCoroutine(ThrowDice());
        }
        else
        {
            auto = false;
            autoLabel.text = "�Զ�ִ��";
        }
    }
    /// <summary>
    /// ��һ������
    /// </summary>
    private IEnumerator ThrowDice()
    {
        diceDone = false;
        do
        {
            // ÿ��������ת10��
            for (int i = 0; i < 10; i++)
            {
                diceCurNum = Random.Range(1, 7);
                dice.text = diceCurNum.ToString();
                yield return new WaitForSeconds(0.1f);
            }

            // �ָ���һ�������ɫ
            players[curPlayerNum].GetComponent<UISprite>().color = Color.white;
            // ѡ���������
            curPlayerNum = (curPlayerNum + diceCurNum - 1) % players.Count;
            players[curPlayerNum].transform.Find("Out").gameObject.SetActive(true);
            string number = players[curPlayerNum].GetComponentInChildren<UILabel>().text;
            players.RemoveAt(curPlayerNum);
            // ����������Һ���
            GameObject outNumber = Instantiate(outNumberPrefab, grid.transform);
            outNumber.GetComponentInChildren<UILabel>().text = number;
            grid.Reposition();

            // ѡ����һ�����
            curPlayerNum = curPlayerNum % players.Count;
            players[curPlayerNum].GetComponent<UISprite>().color = curPlayerColor;
        } while (auto && !GameOver);

        // �ж���Ϸ�Ƿ����
        if (GameOver)
        {
            dice.text = "";
            endPanel.SetActive(true);
        }
        diceDone = true;
    }

    /// <summary>
    /// ������Ϸ����
    /// </summary>
    public void SetTimeScale(UIScrollBar speed)
    {
        Time.timeScale = Mathf.Lerp(1, 10, speed.value);
    }

    /// <summary>
    /// ����Ԥ����ͼƬ��С
    /// </summary>
    private void SetSize()
    {
        // �������ͼƬ��С
        int height = playerArea.height;
        float scale = (float)(GameMgr.Instance.totalNum - GameMgr.minNum) / (GameMgr.maxNum - GameMgr.minNum);
        playerPrefab.GetComponent<UISprite>().width = (int)Mathf.Lerp(height / 8, height / 32, Mathf.Pow(scale, 0.2f));

        // ���ó��ֺ�������
        grid.maxPerLine = (int)Mathf.Pow(GameMgr.Instance.totalNum, 0.5f);
        grid.cellWidth = outNumberArea.width / grid.maxPerLine;
        int titleHeight = outNumberArea.GetComponentInChildren<UILabel>().height * 2;
        grid.cellHeight = (outNumberArea.height - titleHeight) / (GameMgr.Instance.totalNum / grid.maxPerLine + 1);

        // ���ó��ֺ���ͼƬ��С
        outNumberPrefab.GetComponent<UISprite>().width = (int)(grid.cellWidth / 1.1f);
        outNumberPrefab.GetComponent<UISprite>().height = (int)(grid.cellHeight / 1.1f);
    }

    /// <summary>
    /// �������
    /// </summary>
    private void InstantiatePlayers()
    {
        int height = playerArea.height;
        float maxAngle = 360 * Mathf.Max(1, (float)GameMgr.Instance.totalNum / (GameMgr.maxNum / 4));   // �趨��һ��ƽǶ�
        for (int i = 0; i < GameMgr.Instance.totalNum; ++i)
        {
            GameObject player = Instantiate(playerPrefab, playerArea.transform);
            float scale = (float)i / GameMgr.Instance.totalNum;
            float radius = maxAngle == 360 ? height / 4 : Mathf.Lerp(height / 4, height * 4 / 9, scale);    // ���ݻ��ƽǶ��趨���λ�ð뾶
            float angle = Mathf.Lerp(0, maxAngle, scale) * Mathf.Deg2Rad;
            int x = (int)(Mathf.Cos(angle) * radius);
            int y = (int)(Mathf.Sin(angle) * radius);
            player.transform.localPosition = new Vector3(x, y, 0);  // �趨λ��
            player.GetComponentInChildren<UILabel>().text = (i + 1).ToString();
            players.Add(player);
        }
        // �趨��ǰ�����ɫ
        curPlayerNum = 0;
        players[curPlayerNum].GetComponent<UISprite>().color = curPlayerColor;
    }

    protected void Start()
    {
        // �趨��ʾ����
        Tools.LogNull(playerArea, "δ�趨�����ʾ����", true);
        Tools.LogNull(outNumberArea, "δ�趨���ֺ�����ʾ����", true);

        // ��ȡ��Ҷ���
        playerPrefab = Resources.Load<GameObject>("RandomGame/Player");
        Tools.LogNull(playerPrefab, "��ȡ���Ԥ����Playerʧ��", true);
        players = new List<GameObject>();

        // ��ȡ���ֺ������
        outNumberPrefab = Resources.Load<GameObject>("RandomGame/OutNumber");
        Tools.LogNull(outNumberPrefab, "��ȡ���ֺ���Ԥ����OutNumberʧ��", true);
        grid = outNumberArea.GetComponentInChildren<UIGrid>();
        Tools.LogNull(grid, "���ֺ�����ʾ�������Ӷ���û��Grid�ű�", true);

        // �����������
        Tools.LogNull(dice, "δ�趨����dice", true);
        Tools.LogNull(autoLabel, "δ�趨�Զ�ִ�б�ǩautoLabel");
        diceDone = true;
        auto = false;

        inited = true;
        gameObject.SetActive(false);
    }

    protected void OnEnable()
    {
        if (!inited)
            return;
        SetSize();
        InstantiatePlayers();
        curPlayerNum = 0;
        auto = false;
        autoLabel.text = "�Զ�ִ��";
    }

    protected void OnDisable()
    {
        // ɾ�����
        for (int i = 0; i < playerArea.transform.childCount; ++i)
            // �����Ӷ����childCount���������
            Destroy(playerArea.transform.GetChild(i).gameObject);
        // ��Ҫ�ֶ���childCount����
        playerArea.transform.DetachChildren();
        players.Clear();

        // ɾ��������Һ���
        for (int i = 0; i < grid.transform.childCount; ++i)
            Destroy(grid.transform.GetChild(i).gameObject);
        grid.transform.DetachChildren();
    }
}
