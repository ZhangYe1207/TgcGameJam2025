using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Game/Card Database")]
public class CardDatabase : ScriptableObject
{
    public List<Card> allCards;

    private void OnEnable()
    {
        InitializeCardImages();
    }

    // 根据ID获取卡牌
    public Card GetCardById(int cardId)
    {
        return allCards.Find(card => card.cardId == cardId);
    }

    public void InitializeCardImages()
    {
        foreach (Card card in allCards)
        {
            card.cardImage = Resources.Load<Sprite>($"CardSprites/{card.cardName}");
            Debug.Log("card.cardName: " + card.cardName);
            Debug.Log("card.cardId: " + card.cardId);
            Debug.Log("card.cardImage: " + card.cardImage);
        }
    }
}