using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    // 单例模式
    public static PlayerManager Instance { get; private set; }

    // 初始化数据
    public int initMoney = 10000;
    public int initReputation = 0;
    public int initActionPoints = 3; // 初始行动点数
    public List<Card> initCards = new List<Card>();

    public PlayerData playerData;
    public GameObject playerGO;

    private void Awake()
    {
        // 确保单例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化玩家数据
    private void InitializePlayerData()
    {
        playerData = new PlayerData(initMoney, initReputation, initCards);
        // TODO: 可从存档加载数据
        // LoadPlayerData();
    }

    // ===== 金钱管理 =====
    public void AddMoney(int amount)
    {
        playerData.money += amount;
        OnPlayerDataChanged();
    }

    public void RemoveMoney(int amount)
    {
        if (playerData.money >= amount)
        {
            playerData.money -= amount;
            OnPlayerDataChanged();
        }
        else
        {
            Debug.LogError("金钱不足");
        }
    }

    public int GetMoney()
    {
        return playerData.money;
    }

    // ===== 声望管理 =====
    public void AddReputation(int amount)
    {
        playerData.reputation += amount;
        OnPlayerDataChanged();
    }

    public void RemoveReputation(int amount)
    {
        if (playerData.reputation >= amount)
        {
            playerData.reputation -= amount;
            OnPlayerDataChanged();
        }
        else
        {
            Debug.LogError("声望不足");
        }
    }

    public int GetReputation()
    {
        return playerData.reputation;
    }

    // ===== 卡牌管理 =====
    public void AddCard(Card card)
    {
        playerData.cards.Add(card);
        OnPlayerDataChanged();
    }
    
    public void AddCardById(int cardId)
    {
        Card cardToAdd = GameManager.Instance.cardDatabase.GetCardById(cardId);
        if (cardToAdd != null)
        {
            AddCard(cardToAdd);
        }
        else
        {
            Debug.LogError("未找到卡牌ID: " + cardId);
        }
    }

    public void RemoveCardById(int cardId)
    {
        Card cardToRemove = playerData.cards.Find(card => card.cardId == cardId);
        if (cardToRemove != null)
        {
            RemoveCard(cardId);
        }
        else
        {
            Debug.LogError("未找到卡牌ID: " + cardId);
        }
    }


    public void RemoveCard(int cardId)
    {
        Card cardToRemove = playerData.cards.Find(card => card.cardId == cardId);
        if (cardToRemove != null)
        {
            playerData.cards.Remove(cardToRemove);
            OnPlayerDataChanged();
        }
        else
        {
            Debug.LogError("未找到卡牌ID: " + cardId);
        }
    }

    public List<Card> GetAllCards()
    {
        return playerData.cards;
    }

    // ===== 随机事件管理 =====
    public void AddEventToProcess(RandomEvent eventData) {
        playerData.eventsToProcess.Add(eventData);
        Debug.Log("事件加入处理列表: " + eventData.eventId);
        OnPlayerDataChanged();
    }
    
    public void AddEventFinished(string eventId) {
        playerData.eventsFinished.Add(eventId);
        Debug.Log("事件加入完成列表: " + eventId);
        OnPlayerDataChanged();
    }

    public void RemoveEventToProcess(string eventId) {
        playerData.eventsToProcess.RemoveAll(eventData => eventData.eventId == eventId);
        Debug.Log("事件移出处理列表: " + eventId);
        OnPlayerDataChanged();
    }

    public void RemoveEventFinished(string eventId) {
        playerData.eventsFinished.RemoveAll(eId => eId == eventId);
        Debug.Log("事件移出完成列表: " + eventId);
        OnPlayerDataChanged();
    }

    // ===== 行动点管理 =====
    public bool HasActionPoints()
    {
        return playerData.actionPoints > 0;
    }

    public void UseActionPoint()
    {
        if (playerData.actionPoints > 0)
        {
            playerData.actionPoints--;
            OnPlayerDataChanged();
        }
        else
        {
            Debug.LogError("行动点不足");
        }
    }

    public void RefreshActionPoints()
    {
        playerData.actionPoints = initActionPoints;
        OnPlayerDataChanged();
    }

    public int GetActionPoints()
    {
        return playerData.actionPoints;
    }

    // 数据变更回调（可用于更新UI）
    private void OnPlayerDataChanged()
    {
        // 触发事件通知UI更新
        Debug.Log("玩家数据已更新");
        // TODO: SavePlayerData();
    }

    // 保存/加载数据（简化版）
    private void SavePlayerData()
    {
        // 使用PlayerPrefs或JSON等方式保存数据
        string jsonData = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("PlayerData", jsonData);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerData")) {
            string jsonData = PlayerPrefs.GetString("PlayerData");
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        }
    }
}