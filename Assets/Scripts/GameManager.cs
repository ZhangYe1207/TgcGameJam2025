using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentLevel = 0;
    // 这一轮结算时需要结算的项目结果
    public List<ProjectResult> projectResults = new List<ProjectResult>();
    public List<GameProperty> gameProperties;
    public List<Card> handCards;
    public List<string> eventsFinished;
    public List<string> projectFinished;
    public List<string> friends;
    public GameObject playerGO;

    private void Awake()
    {
        // 确保单例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() {
        playerGO = GameObject.FindWithTag("Player");
    }

    // 开始新的一轮
    public void StartNewLevel()
    {
        currentLevel++;
        // TODO: 刷新玩家行动点
        // TODO: 其他每轮开始时的逻辑
    }
}