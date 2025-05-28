using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentLevel = 0;
    // 这一轮结算时需要结算的项目结果
    public List<ProjectResult> projectResults = new List<ProjectResult>();
    public List<GameProperty> baseGameProperties;
    public List<string> HandCards;
    public List<string> EventsFinished;
    public List<string> ProjectFinished;
    public List<string> Friends;
    public GameObject playerGO;
    public GameObject mainUI;
    public GameObject ActionPointUI;
    public GameObject HandCardUI;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject cardUIPrefab;

    private void Awake()
    {
        // 确保单例唯一
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (baseGameProperties == null) {
                baseGameProperties = new List<GameProperty>();
            }
            if (HandCards == null) {
                HandCards = new List<string>();
            }
            if (EventsFinished == null) {
                EventsFinished = new List<string>();
            }
            if (ProjectFinished == null) {
                ProjectFinished = new List<string>();
            }
            if (Friends == null) {
                Friends = new List<string>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() {
        playerGO = GameObject.FindWithTag("Player");
        EffectsCheck();
        ConditionsCheck();
        UpdateMainUI();
    }

    private void EffectsCheck() {
        // 检查数据库中配置的所有效果是否符合格式
    }

    private void ConditionsCheck() {
        // 检查数据库中配置的所有条件是否符合格式
    }   

    // 开始新的一轮
    public void StartNewLevel()
    {
        currentLevel++;
        // TODO: 刷新玩家行动点
        // TODO: 其他每轮开始时的逻辑
    }

    public void OnGameDataChanged() {   
        UpdateMainUI();
    }

    public int GetPropertyCurrentValue(string propertyName) {
        return baseGameProperties.Find(property => property.propertyName == propertyName).currentValue;
    }

    public int GetPropertyMaxValue(string propertyName) {
        return baseGameProperties.Find(property => property.propertyName == propertyName).maxValue;
    }

    public int GetPropertyMinValue(string propertyName) {
        return baseGameProperties.Find(property => property.propertyName == propertyName).minValue;
    }

    private void UpdateMainUI() {
        // TODO: 更新主界面
        UpdateActionPointUI();
        UpdateHandCardUI();
    }

    private void UpdateActionPointUI() {
        int currentActionPoint = GetPropertyCurrentValue("ActionPoints");
        int maxActionPoint = GetPropertyMaxValue("ActionPoints");
        ActionPointUI.transform.Find("ActionPointText").GetComponent<TextMeshProUGUI>().text = $"<size=130%><b>{currentActionPoint}</b></size>/{maxActionPoint}";
    }

    private void UpdateHandCardUI() {
        // Get the ScrollView content
        Transform content = HandCardUI.transform.Find("HandCardScrollView/Viewport/Content");
        if (content == null) {
            Debug.LogError("HandCardUI ScrollView content not found!");
            return;
        }

        // Clear existing cards
        foreach (Transform child in content) {
            Destroy(child.gameObject);
        }

        // Add cards from hand
        foreach (string cardId in HandCards) {
            Card cardData = DatabaseManager.Instance.cardDatabase.GetCardById(cardId);
            if (cardData != null) {
                // Instantiate card UI
                GameObject cardGO = Instantiate(cardUIPrefab, content);
                
                // Set card image
                Image cardImage = cardGO.GetComponent<Image>();
                if (cardImage != null && cardData.cardImage != null) {
                    cardImage.sprite = cardData.cardImage;
                    // Add random rotation
                    float randomRotation = Random.Range(-5f, 5f);
                    cardImage.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
                }
            }
        }
    }

    private void OnCardClicked(Card card) {
        // TODO: Implement card click handling
        Debug.Log($"Card clicked: {card.cardName}");
    }
}