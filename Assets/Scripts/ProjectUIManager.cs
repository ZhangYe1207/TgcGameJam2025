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
    [SerializeField] private Project projectData;
    [SerializeField] private Sprite cardSlotSprite;

    private ProjectHandler projectHandler;
    
    [Header("UI Prefabs")]
    [SerializeField] private GameObject cardUIPrefab;

    private void Awake() {
        projectUI.SetActive(false);
        Button[] cardSlotButtons = cardSlots.GetComponentsInChildren<Button>();
        foreach (Button cardSlotButton in cardSlotButtons) {
            cardSlotButton.onClick.AddListener(removeCard);
        }
    }

    public void SetProjectHandler(ProjectHandler handler) {
        projectHandler = handler;
        projectData = handler.projectData;
        ResetDicesAndCardSlots();
    }

    public void ResetDicesAndCardSlots() {
        // 设置手上的骰子数量，当前写死2个
        GameManager.Instance.SetPropertyCurrentValue("HaveDices", 2);
        GameManager.Instance.SetPropertyCurrentValue("NeedDices", projectData.initNeedDices);
        GameManager.Instance.SetPropertyCurrentValue("CardSlots", projectData.initCardSlots);
    }

    public void ShowProject() {
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

        projectUI.SetActive(true);
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
        needDicesText.text = $"Need <size=130%><b>{GameManager.Instance.GetPropertyCurrentValue("NeedDices")}</b></size>";
        haveDicesText.text = $"Have <size=130%><b>{GameManager.Instance.GetPropertyCurrentValue("HaveDices")}</b></size>";
    }

    public void UpdateCardSlotUI() {
        Debug.Log("update card slot ui");
        int cardSlotsCount = GameManager.Instance.GetPropertyCurrentValue("CardSlots");
        Button[] cardSlotButtons = cardSlots.GetComponentsInChildren<Button>();
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
            // Add card back to hand
            GameManager.Instance.HandCards.Add(cardDataHolder.cardData.cardId);
            
            // Remove card from slot
            GameManager.Instance.currentPlacedCards.Remove(cardDataHolder.cardData);
            cardDataHolder.cardData = null;
            
            // 每次移除卡牌后，重新把所有放置的卡牌结算一遍
            ResetDicesAndCardSlots();
            GameManager.Instance.ExecuteCardEffects();
            UpdateCardSlotUI();
            UpdateDicesUI();
            GameManager.Instance.UpdateHandCardUI();
        }   
    }
}
