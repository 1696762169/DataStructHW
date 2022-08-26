using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayingControl : MonoBehaviour
{
    [Tooltip("玩家显示区域")]
    public UIWidget playerArea;
    [Tooltip("当前玩家颜色")]
    public Color curPlayerColor;
    // 玩家对象预设体
    private GameObject playerPrefab;
    // 玩家列表
    private List<GameObject> players;
    // 当前玩家编号(从0开始)
    private int curPlayerNum;

    [Tooltip("随机显示数字(充当骰子)")]
    public UILabel dice;
    // 骰子当前的数字
    private int diceCurNum;
    // 一轮游戏执行完毕
    private bool diceDone;

    [Tooltip("自动执行按钮标签")]
    public UILabel autoLabel;
    // 自动执行
    private bool auto;

    [Tooltip("出局号码显示区域")]
    public UIWidget outNumberArea;
    // 出局号码预设体
    private GameObject outNumberPrefab;
    private UIGrid grid;

    // 初始化标识
    private bool inited;

    // 游戏结束面板
    private GameObject endPanel => GameMgr.Instance.endPanel;
    // 游戏结束标识
    private bool GameOver => players.Count <= 1;

    /// <summary>
    /// 进行一轮游戏
    /// </summary>
    public void Play()
    {
        if (diceDone)
            StartCoroutine(ThrowDice());
    }
    /// <summary>
    /// 自动进行游戏
    /// </summary>
    public void AutoPlay()
    {
        if (!auto)
        {
            auto = true;
            autoLabel.text = "停止自动";
            if (diceDone)
                StartCoroutine(ThrowDice());
        }
        else
        {
            auto = false;
            autoLabel.text = "自动执行";
        }
    }
    /// <summary>
    /// 扔一次骰子
    /// </summary>
    private IEnumerator ThrowDice()
    {
        diceDone = false;
        do
        {
            // 每次扔骰子转10次
            for (int i = 0; i < 10; i++)
            {
                diceCurNum = Random.Range(1, 7);
                dice.text = diceCurNum.ToString();
                yield return new WaitForSeconds(0.1f);
            }

            // 恢复上一个玩家颜色
            players[curPlayerNum].GetComponent<UISprite>().color = Color.white;
            // 选择死亡玩家
            curPlayerNum = (curPlayerNum + diceCurNum - 1) % players.Count;
            players[curPlayerNum].transform.Find("Out").gameObject.SetActive(true);
            string number = players[curPlayerNum].GetComponentInChildren<UILabel>().text;
            players.RemoveAt(curPlayerNum);
            // 生成死亡玩家号码
            GameObject outNumber = Instantiate(outNumberPrefab, grid.transform);
            outNumber.GetComponentInChildren<UILabel>().text = number;
            grid.Reposition();

            // 选中下一个玩家
            curPlayerNum = curPlayerNum % players.Count;
            players[curPlayerNum].GetComponent<UISprite>().color = curPlayerColor;
        } while (auto && !GameOver);

        // 判断游戏是否结束
        if (GameOver)
        {
            dice.text = "";
            endPanel.SetActive(true);
        }
        diceDone = true;
    }

    /// <summary>
    /// 更改游戏速率
    /// </summary>
    public void SetTimeScale(UIScrollBar speed)
    {
        Time.timeScale = Mathf.Lerp(1, 10, speed.value);
    }

    /// <summary>
    /// 设置预设体图片大小
    /// </summary>
    private void SetSize()
    {
        // 设置玩家图片大小
        int height = playerArea.height;
        float scale = (float)(GameMgr.Instance.totalNum - GameMgr.minNum) / (GameMgr.maxNum - GameMgr.minNum);
        playerPrefab.GetComponent<UISprite>().width = (int)Mathf.Lerp(height / 8, height / 32, Mathf.Pow(scale, 0.2f));

        // 设置出局号码网格
        grid.maxPerLine = (int)Mathf.Pow(GameMgr.Instance.totalNum, 0.5f);
        grid.cellWidth = outNumberArea.width / grid.maxPerLine;
        int titleHeight = outNumberArea.GetComponentInChildren<UILabel>().height * 2;
        grid.cellHeight = (outNumberArea.height - titleHeight) / (GameMgr.Instance.totalNum / grid.maxPerLine + 1);

        // 设置出局号码图片大小
        outNumberPrefab.GetComponent<UISprite>().width = (int)(grid.cellWidth / 1.1f);
        outNumberPrefab.GetComponent<UISprite>().height = (int)(grid.cellHeight / 1.1f);
    }

    /// <summary>
    /// 创建玩家
    /// </summary>
    private void InstantiatePlayers()
    {
        int height = playerArea.height;
        float maxAngle = 360 * Mathf.Max(1, (float)GameMgr.Instance.totalNum / (GameMgr.maxNum / 4));   // 设定玩家环绕角度
        for (int i = 0; i < GameMgr.Instance.totalNum; ++i)
        {
            GameObject player = Instantiate(playerPrefab, playerArea.transform);
            float scale = (float)i / GameMgr.Instance.totalNum;
            float radius = maxAngle == 360 ? height / 4 : Mathf.Lerp(height / 4, height * 4 / 9, scale);    // 根据环绕角度设定玩家位置半径
            float angle = Mathf.Lerp(0, maxAngle, scale) * Mathf.Deg2Rad;
            int x = (int)(Mathf.Cos(angle) * radius);
            int y = (int)(Mathf.Sin(angle) * radius);
            player.transform.localPosition = new Vector3(x, y, 0);  // 设定位置
            player.GetComponentInChildren<UILabel>().text = (i + 1).ToString();
            players.Add(player);
        }
        // 设定当前玩家颜色
        curPlayerNum = 0;
        players[curPlayerNum].GetComponent<UISprite>().color = curPlayerColor;
    }

    protected void Start()
    {
        // 设定显示区域
        Tools.LogNull(playerArea, "未设定玩家显示区域", true);
        Tools.LogNull(outNumberArea, "未设定出局号码显示区域", true);

        // 获取玩家对象
        playerPrefab = Resources.Load<GameObject>("RandomGame/Player");
        Tools.LogNull(playerPrefab, "获取玩家预设体Player失败", true);
        players = new List<GameObject>();

        // 获取出局号码对象
        outNumberPrefab = Resources.Load<GameObject>("RandomGame/OutNumber");
        Tools.LogNull(outNumberPrefab, "获取出局号码预设体OutNumber失败", true);
        grid = outNumberArea.GetComponentInChildren<UIGrid>();
        Tools.LogNull(grid, "出局号码显示区域及其子对象没有Grid脚本", true);

        // 检测其它对象
        Tools.LogNull(dice, "未设定骰子dice", true);
        Tools.LogNull(autoLabel, "未设定自动执行标签autoLabel");
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
        autoLabel.text = "自动执行";
    }

    protected void OnDisable()
    {
        // 删除玩家
        for (int i = 0; i < playerArea.transform.childCount; ++i)
            // 销毁子对象后childCount并不会更新
            Destroy(playerArea.transform.GetChild(i).gameObject);
        // 需要手动将childCount清零
        playerArea.transform.DetachChildren();
        players.Clear();

        // 删除出局玩家号码
        for (int i = 0; i < grid.transform.childCount; ++i)
            Destroy(grid.transform.GetChild(i).gameObject);
        grid.transform.DetachChildren();
    }
}
