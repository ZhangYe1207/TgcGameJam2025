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