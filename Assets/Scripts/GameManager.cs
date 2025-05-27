using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public CardDatabase cardDatabase;
    public EventDatabase eventDatabase;
    public ProjectDatabase projectDatabase;
    public int currentLevel = 0;
    // 这一轮结算时需要结算的项目结果
    public List<ProjectResult> projectResults = new List<ProjectResult>();
    public int currentActionPoints = 0;
    public List<GameProperty> gameProperties = new List<GameProperty>();

    private void Awake()
    {
        // 确保单例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            cardDatabase = Resources.Load<CardDatabase>("Databases/CardDatabase");
            eventDatabase = Resources.Load<EventDatabase>("Databases/EventDatabase");
            projectDatabase = Resources.Load<ProjectDatabase>("Databases/ProjectDatabase");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 开始新的一轮
    public void StartNewLevel()
    {
        currentLevel++;
        // 刷新玩家行动点
        PlayerManager.Instance.RefreshActionPoints();
        // TODO: 其他每轮开始时的逻辑
    }
}