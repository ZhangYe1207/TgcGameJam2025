using System.Collections.Generic;
using UnityEngine;

// 卡牌数据类
[System.Serializable]
public class Card
{
    public string cardName;
    public int cardId;
    public string description;
    public Sprite cardImage;
    // 其他卡牌属性...
}

// 玩家数据类
[System.Serializable]
public class PlayerData
{
    public int money;       // 金钱
    public int reputation;  // 声望
    public List<Card> cards; // 卡牌列表

    // 构造函数
    public PlayerData()
    {
        money = 0;
        reputation = 0;
        cards = new List<Card>();
    }

    public PlayerData(int money, int reputation, List<Card> cards)
    {
        this.money = money;
        this.reputation = reputation;
        this.cards = cards;
    }
}

// 事件类型枚举
public enum EventType
{
    Resoursce,
    PreviewableProject,
    UnpreviewableProject
}
 
// 事件选择结果
[System.Serializable]
public class EventResult
{
    public string description;     // 结果描述
    public int moneyChange;        // 金钱变化
    public int reputationChange;   // 声望变化
    public int[] addCardIds;     // 添加的卡牌ID
    public int[] removeCardIds;  // 移除的卡牌ID
}

// 事件前置条件
[System.Serializable]
public class EventPrerequisite
{
    public int minReputation;   // 最小声望
    public int maxReputation;   // 最大声望
    public int minMoney;        // 最小金钱
    public int maxMoney;        // 最大金钱
    public int minLevel;        // 最早出现的关卡
    public int maxLevel;        // 最晚出现的关卡
    public string[] requiredEventIds; // 前置事件ID
}

// 随机事件基类
[System.Serializable]
public class RandomEvent
{
    public string eventId;         // 事件唯一ID
    public string title;           // 事件标题
    public string description;     // 事件描述
    public EventType eventType;    // 事件类型
    public Sprite eventImage;      // 事件图片
    public EventResult[] results;  // 可选结果
    public EventPrerequisite prerequisite; // 前置条件
}