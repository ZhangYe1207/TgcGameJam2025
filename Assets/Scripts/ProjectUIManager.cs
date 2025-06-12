using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProjectUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject projectUI;
    [SerializeField] private Button investButton;
    [SerializeField] private Button laterButton;
    [SerializeField] private TextMeshProUGUI needDicesText;
    [SerializeField] private TextMeshProUGUI haveDicesText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI projectTitleText;
    [SerializeField] private Image projectImage;
    [SerializeField] private GameObject cardSlots;
    [SerializeField] private GameObject resultUI;
    [SerializeField] private Button resultConfirmButton;
    [SerializeField] private Sprite cardSlotSprite;

    private ProjectHandler projectHandler;
    
    [Header("UI Prefabs")]
    [SerializeField] private GameObject cardUIPrefab;

    [SerializeField] private Project projectData;

    private void Awake() {
        projectUI.SetActive(false);
        Button[] cardSlotButtons = cardSlots.GetComponentsInChildren<Button>();
        foreach (Button cardSlotButton in cardSlotButtons) {
            cardSlotButton.onClick.AddListener(removeCard);
        }
        investButton.onClick.AddListener(Invest);
        laterButton.onClick.AddListener(Later);
        resultConfirmButton.onClick.AddListener(ResultConfirm);
    }

    public void SetProjectHandler(ProjectHandler handler) {
        projectHandler = handler;
        projectData = handler.projectData;
        ResetDicesAndCardSlots();
    }

    public void ResetDicesAndCardSlots() {
        // 初始骰子和慷慨值挂钩，公式固定为2+n
        GameManager.Instance.SetPropertyCurrentValue("HaveDices", 2);
        EffectExecutor.ExecuteEffect($"HaveDices += {GameManager.Instance.GetPropertyCurrentValue("Generosity")}");
        GameManager.Instance.SetPropertyCurrentValue("NeedDices", projectData.initNeedDices);
        GameManager.Instance.SetPropertyCurrentValue("CardSlots", projectData.initCardSlots);
    }

    public void ShowProject() {
        projectUI.SetActive(true);

        // disable resultUI
        resultUI.SetActive(false);
        resultConfirmButton.gameObject.SetActive(false);
        // set project info
        projectTitleText.text = projectData.title;
        descriptionText.text = projectData.description;
        if (projectData.projectImage != null)
        {
            projectImage.sprite = projectData.projectImage;
            projectImage.gameObject.SetActive(true);
        }
        else
        {
            projectImage.gameObject.SetActive(false);
        }
        // 可预览事件才显示laterbutton
        laterButton.gameObject.SetActive(projectData.isPreviewable);
        // 骰子相关ui设置
        UpdateDicesUI();
        // 卡槽相关ui设置
        ClearCardSlotData();
        UpdateCardSlotUI();

        GameManager.Instance.isOnProjectUI = true;
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = true;
        GameManager.Instance.UpdateHandCardUI();
    }

    public void ClearCardSlotData() {
        foreach (Transform slot in cardSlots.transform) {
            slot.GetComponent<CardDataHolder>().cardData = null;
        }
    }

    public void UpdateDicesUI() {
        if (!needDicesText.gameObject.activeSelf) {
            needDicesText.gameObject.SetActive(true);
        }
        if (!haveDicesText.gameObject.activeSelf) {
            haveDicesText.gameObject.SetActive(true);
        }
        needDicesText.text = $"Need Dices: <size=130%><b>{GameManager.Instance.GetPropertyCurrentValue("NeedDices")}</b></size>";
        haveDicesText.text = $"Have Dices: <size=130%><b>{GameManager.Instance.GetPropertyCurrentValue("HaveDices")}</b></size>";
    }

    public void UpdateCardSlotUI() {
        int cardSlotsCount = GameManager.Instance.GetPropertyCurrentValue("CardSlots");
        Debug.Log($"update card slot ui, card slot number: {cardSlotsCount}");
        Button[] cardSlotButtons = cardSlots.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < cardSlotButtons.Length; i++) {
            if (i < cardSlotsCount) {
                cardSlotButtons[i].gameObject.SetActive(true);
            }
            else {
                cardSlotButtons[i].gameObject.SetActive(false);
            }
        }
        foreach (Transform slot in cardSlots.transform) {
            if (slot.GetComponent<CardDataHolder>().cardData == null) {
                slot.GetComponent<Image>().sprite = cardSlotSprite;
            }
            else {
                slot.GetComponent<Image>().sprite = slot.GetComponent<CardDataHolder>().cardData.cardImage;
            }
        }
    }

    private void removeCard() {
        // Get the clicked slot
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (clickedButton == null) return;

        // Check if slot has a card
        CardDataHolder cardDataHolder = clickedButton.GetComponent<CardDataHolder>();
        if (cardDataHolder != null && cardDataHolder.cardData != null) {
            RemoveCardFromSlot(cardDataHolder);
                
            // 每次移除卡牌后，重新把所有放置的卡牌结算一遍
            ResetDicesAndCardSlots();
            GameManager.Instance.ExecuteCardEffects();
            UpdateCardSlotUI();
            UpdateDicesUI();
            GameManager.Instance.UpdateHandCardUI();
        }   
    }

    private void RemoveCardFromSlot(CardDataHolder cardDataHolder) {
        // Add card back to hand
        GameManager.Instance.HandCards.Add(cardDataHolder.cardData.cardId);
        // Remove card from slot
        GameManager.Instance.currentPlacedCards.Remove(cardDataHolder.cardData);
        cardDataHolder.cardData = null;
    }

    private void Invest() {
        // 检查是否满足投资条件, mustPlaceCards中的卡牌是否都放置了
        int placeAmount = 0;
        foreach (Card c in GameManager.Instance.currentPlacedCards) {
            if (c.cardType == CardType.Money) {
                placeAmount += c.amount;
            }
        }
        if (placeAmount < projectData.needMoney) {
            GameManager.Instance.PromptUI.ShowOkPrompt("This project need at least \n<color=yellow><b>\"" + projectData.needMoney + " million money\"</color></b>");
            return;
        }
        foreach (string cardId in projectData.mustPlaceCards) {
            if (!GameManager.Instance.currentPlacedCards.Exists(card => card.cardId == cardId)) {
                Card card = DatabaseManager.Instance.cardDatabase.GetCardById(cardId);
                GameManager.Instance.PromptUI.ShowOkPrompt("You must place the card \"" + card.cardName + "\" to invest in this project.");
                return;
            }
        }
        // 如果满足，则进行骰骰子模拟
        int[] dices = GameManager.Instance.RollDices();
        // 获取结果
        int resultIndex = GameManager.Instance.GetProjectResultIndex(dices);
        ProjectResult result = projectData.results[resultIndex];
        Debug.Log("Invest result: " + result.description);
        // 执行结果
        foreach (EffectData effect in result.immediateEffects) {
            EffectExecutor.ExecuteEffect(effect.effectCode);
        }
        foreach (DelayedEffectData effect in result.delayedEffects) {
            GameManager.Instance.DelayedEffects.Add(effect);
        }
        // 更新UI
        resultUI.SetActive(true);
        resultUI.transform.GetComponentsInChildren<TextMeshProUGUI>(true)[0].text = result.description;
        Image resultImage = resultUI.transform.GetComponentsInChildren<Image>(true)[1];
        if (result.resultImage != null) {
            resultImage.sprite = result.resultImage;
        } else {
            resultImage.gameObject.SetActive(false);
        }
        GameManager.Instance.OnGameDataChanged();
        resultConfirmButton.gameObject.SetActive(true);
        laterButton.gameObject.SetActive(false);
        needDicesText.gameObject.SetActive(false);
        haveDicesText.gameObject.SetActive(false);
        // 
    }

    private void ResultConfirm() {
        Debug.Log("ResultConfirm");
        // 关闭UI
        resultUI.SetActive(false);
        resultConfirmButton.gameObject.SetActive(false);
        laterButton.gameObject.SetActive(true);
        projectUI.SetActive(false);
        GameManager.Instance.isOnProjectUI = false;
        projectHandler.ConfirmInvestment();
    }

    private void Later() {
        // 清空卡槽，返还手牌
        foreach (Transform slot in cardSlots.transform) {
            if (slot.GetComponent<CardDataHolder>().cardData != null) {
                RemoveCardFromSlot(slot.GetComponent<CardDataHolder>());
            }
        }
        ResetDicesAndCardSlots();
        UpdateCardSlotUI();
        UpdateDicesUI();
        GameManager.Instance.UpdateHandCardUI();
        GameManager.Instance.currentPlacedCards.Clear();

        // 关闭UI
        projectUI.SetActive(false);
        GameManager.Instance.isOnProjectUI = false;
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
    }
}
