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
    public List<RandomEvent> eventsToProcess; // 需要处理的随机事件列表
    public List<string> eventsFinished; // 已完成随机事件列表

    // 构造函数
    public PlayerData()
    {
        money = 0;
        reputation = 0;
        cards = new List<Card>();
        eventsToProcess = new List<RandomEvent>();
        eventsFinished = new List<string>();
    }

    public PlayerData(int money, int reputation, List<Card> cards)
    {
        this.money = money;
        this.reputation = reputation;
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
    public int moneyChange;        // 金钱变化
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
    public int minMoney;        // 最小金钱
    public int maxMoney;        // 最大金钱
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

    [Header("实际成功率配置")]
    [Tooltip("实际结算时的基础成功率")]
    [Range(0f, 1f)] public float baseSuccessRate = 0.5f; 
    
    
    [Tooltip("实际结算时的成功率计算公式")]
    public SuccessRateFormula formulaType = SuccessRateFormula.Linear;
    public float formulaParamA = 0.0001f;
    public float formulaParamB = 0.01f;

    [Header("玩家看到的基础成功率配置，可能和实际结算时的成功率不同，用于UI显示")]
    [Tooltip("玩家看到的基础成功率")]
    [Range(0f, 1f)] public float showBaseSuccessRate = 0.5f; 
    [Tooltip("玩家看到的基础成功率计算公式")]
    public SuccessRateFormula showFormulaType = SuccessRateFormula.Linear;
    public float showFormulaParamA = 0.0001f;
    public float showFormulaParamB = 0.01f;

    // 存储自定义公式的委托
    private System.Func<float, float, float, float, float> customSuccessRateFormula;
    private System.Func<float, float, float, float, float> customShowSuccessRateFormula;

    // 预设的成功率计算公式类型
    public enum SuccessRateFormula
    {
        Linear,      // 线性增长
        Logarithmic, // 对数增长
        Exponential, // 指数增长
        Custom       // 自定义公式
    }

    // 设置自定义公式
    public void SetCustomFormula(System.Func<float, float, float, float, float> formula)
    {
        customSuccessRateFormula = formula;
        formulaType = SuccessRateFormula.Custom;
    }

    // 计算基于投资金额的成功率
    public float CalculateSuccessRate(float investment, float reputation)
    {
        switch (formulaType)
        {
            case SuccessRateFormula.Linear:
                return Mathf.Clamp01(baseSuccessRate + formulaParamA * investment + formulaParamB * reputation);
                
            case SuccessRateFormula.Logarithmic:
                return Mathf.Clamp01(baseSuccessRate + formulaParamA * Mathf.Log(investment + 1) + formulaParamB * reputation);
                
            case SuccessRateFormula.Exponential:
                return Mathf.Clamp01(baseSuccessRate + formulaParamA * (Mathf.Exp(investment) - 1) + formulaParamB * reputation);
                
            case SuccessRateFormula.Custom:
                Debug.LogError("自定义公式未设置!");
                return baseSuccessRate; 
                    
            default:
                return baseSuccessRate;
        }
    }
}
