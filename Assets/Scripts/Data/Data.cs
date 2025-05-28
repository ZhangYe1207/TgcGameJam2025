using System.Collections.Generic;
using UnityEngine;

// 卡牌数据类
[System.Serializable]
public class Card
{
    public string cardName;
    public string cardId;
    public string description;
    public CardType cardType;
    public Sprite cardImage;
    public List<EffectData> cardEffects;
    // 其他卡牌属性...
}

[System.Serializable]
public enum CardType
{
    Money,
    Function,
    Friend,
}

[System.Serializable]
public class EffectData
{
    public string effectCode;
    [TextArea] public string explanation;
}   

[System.Serializable]
public class ConditionData
{
    public string conditionCode;
    [Tooltip("条件不满足时显示的提示信息")]
    [TextArea] public string failedMessage;
}

[System.Serializable]
public enum GamePropertyOutOfRangeHandlePolicy
{
    Clamp,
    Error,
}

[System.Serializable]
public class GameProperty
{
    public string propertyName;
    public int currentValue;
    public int maxValue;
    public int minValue;
    public GamePropertyOutOfRangeHandlePolicy outOfRangeHandlePolicy;
    [TextArea] public string explanation;
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
    public Sprite resultImage;
    public List<EffectData> eventEffects;
}

// 事件前置条件
[System.Serializable]
public class EventPrerequisite
{
    public string description;
    [Tooltip("Condition之间是And的关系，全部满足则这条Prerequisite满足")]
    public ConditionData[] conditions; 
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
    [Tooltip("Prerequisite可以有多个，如果满足任意一个则可以触发事件")]
    public EventPrerequisite[] prerequisites; // 前置条件
}

// 项目结果
[System.Serializable]
public class ProjectResult
{
    public string description;     // 结果描述
    public EffectData[] effects;
}

// 项目前置条件
[System.Serializable]
public class ProjectPrerequisite
{
    public string description;
    [Tooltip("Condition之间是And的关系，全部满足则这条Prerequisite满足")]
    public ConditionData[] conditions; 
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
    public List<string> mustPlaceCards; // 必须放置的卡牌ID
    public int initNeedDices; // 初始需要的正面骰子数
    [Tooltip("Prerequisite可以有多个，如果满足任意一个则可以触发事件")]
    public ProjectPrerequisite[] prerequisites; // 前置条件
}
