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
    public int reputation;  // 声望
    public int actionPoints; // 行动点
    public List<Card> cards; // 卡牌列表
    public List<RandomEvent> eventsToProcess; // 需要处理的随机事件列表
    public List<string> eventsFinished; // 已完成随机事件列表

    // 构造函数
    public PlayerData()
    {
        reputation = 0;
        actionPoints = 0;
        cards = new List<Card>();
        eventsToProcess = new List<RandomEvent>();
        eventsFinished = new List<string>();
    }

    public PlayerData(int reputation, List<Card> cards)
    {
        this.reputation = reputation;
        this.actionPoints = 3; // 默认初始行动点
        this.cards = cards;
        eventsToProcess = new List<RandomEvent>();
        eventsFinished = new List<string>();
    }
}

// 事件类型枚举
public enum EventType
{
    Resource
}
 
// 事件选择结果
[System.Serializable]
public class EventResult
{
    public string description;     // 结果描述
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
    public bool isPreviewable;     // 是否可预览
    public Sprite eventImage;      // 事件图片
    public EventResult[] results;  // 可选结果
    public EventPrerequisite prerequisite; // 前置条件
}

// 项目结果
[System.Serializable]
public class ProjectResult
{
    public string resultBrief;     // 结果简要
    public string description;     // 结果描述
    public int reputationChange;   // 声望变化
    public int[] addCardIds;     // 添加的卡牌ID
    public int[] removeCardIds;  // 移除的卡牌ID
    [Tooltip("在多少关之后结算，如果为0则在当前关卡结束后结算")]
    public int numLevelsToTakeEffect; 
    [Tooltip("如果这里有值，会自动触发下一个项目而非结算")]
    public string nextProjectId;   
}

// 项目前置条件
[System.Serializable]
public class ProjectPrerequisite
{
    public int minReputation;   // 最小声望
    public int maxReputation;   // 最大声望
    public int minLevel;        // 最早出现的关卡
    public int maxLevel;        // 最晚出现的关卡
    public string[] requiredProjectIds; // 前置项目ID
}

[System.Serializable]
public class Project
{
    public string projectId;
    public string title;
    public string description;
    public Sprite projectImage;
    public bool isPreviewable;     // 是否可预览
    public ProjectResult[] results;  // 可选结果
    public ProjectPrerequisite prerequisite; // 前置条件
}
