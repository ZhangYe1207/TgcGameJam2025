using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

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
    public int amount;
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
public class DelayedEffectData
{
    public string effectCode;
    [Header("延迟等级，0为立即生效，1为下一回合生效，2为下下回合生效，以此类推")]
    public int delayedLevel;
    [Header("在延迟效果生效时显示的文字，也就是轮次结算时展示的文字")]
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
public enum RandomEventType
{
    Resource
}
 
// 事件选择结果
[System.Serializable]
public class EventResult
{
    [TextArea] public string description;     // 结果描述
    public Sprite resultImage;
    [Header("立即生效的Effect")]
    public List<EffectData> immediateEffects;
    [Header("延迟生效的Effect")]
    public List<DelayedEffectData> delayedEffects;
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
    public RandomEventType eventType;    // 事件类型
    public bool isPreviewable;     // 是否可预览
    public Sprite eventImage;      // 事件图片
    public EventResult[] results;  // 可选结果
    [Tooltip("事件出现的关卡，第一关是0，第二关是1， 依次类推")]
    public List<int> levels;              // 事件出现的关卡
    [Tooltip("Prerequisite可以有多个，如果满足任意一个则可以触发事件")]
    public EventPrerequisite[] prerequisites; // 前置条件
}


[System.Serializable]
public class NPC 
{
    public string npcName;
    [Header("NPC 出现的关卡")]
    public int level;
    // [Header("NPC 出现的条件，Condition之间是And的关系。\n这个条件会在关卡初始化的时候检查，在这个时间点满足条件的本关NPC才会出现")]
    // public EventPrerequisite prerequisites;
    [Header("把Conversations下面配的会话名称配进来，比如ExampleConv_Lilian")]
    public string npcConversationName;
    [Header("在这里配result，然后在DialogueEditor里面配每个对话对应的result index")]
    public NPCResult[] results;
}

[System.Serializable]
public class NPCResult
{
    public string description;
    [Header("立即生效的Effect")]
    public List<EffectData> immediateEffects;
    [Header("延迟生效的Effect")]
    public List<DelayedEffectData> delayedEffects;
}

// 项目结果
[System.Serializable]
public class ProjectResult
{
    [TextArea] public string description;     // 结果描述
    public Sprite resultImage;
    [Header("立即生效的Effect")]
    public List<EffectData> immediateEffects;
    [Header("延迟生效的Effect")]
    public List<DelayedEffectData> delayedEffects;
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
    [Tooltip("事件出现的关卡，第一关是0，第二关是1， 依次类推")]
    public int level = 0;     
    [Tooltip("当前结果0为失败，结果1为成功，后面的预留，未实现")]
    public ProjectResult[] results;  // 可选结果
    public List<string> mustPlaceCards; // 必须放置的卡牌ID
    [Tooltip("至少需要的钱，所有放置的money card的amount之和要大于等于这个值")]
    public int needMoney;
    public int initNeedDices; // 初始需要的正面骰子数
    public int initCardSlots; // 初始卡槽数
    [Tooltip("Prerequisite可以有多个，如果满足任意一个则可以触发事件")]
    public ProjectPrerequisite[] prerequisites; // 前置条件
}
