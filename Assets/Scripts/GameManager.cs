using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentLevel = 0;
    // 这一轮结算时需要结算的项目结果
    public List<ProjectResult> projectResults = new List<ProjectResult>();
    public GameObject locationsGO;
    [Header("每一关的初始行动点")]
    public List<int> initActionPoints;
    [Header("每一关的初始手牌, 卡牌ID之间用逗号分隔")]
    public List<string> initHandCards;
    [Header("基本的游戏数值属性")]
    public List<GameProperty> baseGameProperties;
    [Header("List Game Properties. Can be used in Effects and Conditions")]
    public List<string> HandCards;
    public List<string> EventsFinished;
    public List<string> ProjectFinished;
    public List<string> Friends;
    [Header("Delayed Effects. 延迟生效的效果，每一轮结束后结算")]
    public List<DelayedEffectData> DelayedEffects;

    [Header("Game States")]
    public GameObject playerGO;
    public List<Card> currentPlacedCards;
    public bool isOnProjectUI;

    [Header("UI References")]
    public GameObject MainCanvas;
    public GameObject mainUI;
    public GameObject ActionPointUI;
    public GameObject ProfessionalityUI;
    public GameObject GenerosityUI;
    public GameObject HandCardUI;
    public ProjectUIManager ProjectUI;
    public PromptUIManager PromptUI;
    public Button NextDayButton;
    public GameObject DailyReportUI;
    public Button DailyReportConfirmButton;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private GameObject dailyResultItemPrefab;
    [Header("Location Prefabs")]
    [SerializeField] private GameObject eventLocationPrefab;
    [SerializeField] private GameObject projectLocationPrefab;
    [Header("Next Level Controller")]
    [SerializeField] private NextLevelController nextLevelController;


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
            if (DelayedEffects == null) {
                DelayedEffects = new List<DelayedEffectData>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start() {
        playerGO = GameObject.FindWithTag("Player");
        MainCanvas.SetActive(true);
        EffectsCheck();
        ConditionsCheck();
        SetupNewLevel();
        UpdateMainUI();
        NextDayButton.onClick.AddListener(NextDay);
        DailyReportConfirmButton.onClick.AddListener(DailyReportConfirm);
        DailyReportUI.SetActive(false);
    }

    private void EffectsCheck() {
        // 检查数据库中配置的所有效果是否符合格式
    }

    private void ConditionsCheck() {
        // 检查数据库中配置的所有条件是否符合格式
    }   

    private void DailyReportConfirm() {
        DailyReportUI.SetActive(false);
        StartCoroutine(nextLevelController.SwitchWithOverview(currentLevel));
    }

    private void NextDay() {
        // Delay Effects处理
        List<string> descriptions = new List<string>();
        for (int i = DelayedEffects.Count - 1; i >= 0; i--) {
            DelayedEffectData effect = DelayedEffects[i];
            if (effect.delayedLevel == 0) {
                EffectExecutor.ExecuteEffect(effect.effectCode);
                descriptions.Add(effect.explanation);
                DelayedEffects.RemoveAt(i);
            } else {
                effect.delayedLevel--;
            }
        }
        // 结算ui展示
        DailyReportUI.SetActive(true);
        SetupDailyReportUI(descriptions);
        // 清理本轮的缓存数据，如果有？
        // 下一轮数据生成&更新
        currentLevel++;
        SetupNewLevel();
        UpdateMainUI();
        
    }

    private void SetupNewLevel() {
        // 行动点设置
        SetPropertyCurrentValue("ActionPoints", initActionPoints[currentLevel]);
        SetPropertyMaxValue("ActionPoints", initActionPoints[currentLevel]);
        // 初始卡牌发放
        string s = initHandCards[currentLevel];
        Debug.Log($"Adding init cards: {s}");
        if (s != null && s != "") {
            string[] cIds = s.Split(",");
            foreach (string cId in cIds) {
                HandCards.Add(cId.Trim());
                Debug.Log($"Added {cId} to HandCards.");
            }
        }
        
        // 捞取当前关卡所有事件/项目
        List<RandomEvent> events = DatabaseManager.Instance.eventDatabase.GetEventsByLevel(currentLevel);
        List<Project> projects = DatabaseManager.Instance.projectDatabase.GetProjectsByLevel(currentLevel);
        int cnt = events.Count + projects.Count;
        // 随机获取location
        List<Location> locations = DatabaseManager.Instance.locationDatabase.GetRandomLocationsByLevel(currentLevel, cnt);
        if (locations.Count < cnt) {
            Debug.LogWarning("This level doesn't have enough locations, will random pick events and projects.");
            // TODO 随机选择
        } else {
            for (int i = 0; i < cnt; i++) {
                if (i < events.Count) {
                    SetupEventLocation(events[i], locations[i]);
                } else {
                    SetupProjectLocation(projects[i - events.Count], locations[i]);
                }
            }
        }
    }

    private void SetupEventLocation(RandomEvent e, Location location) {
        GameObject locGO = Instantiate(eventLocationPrefab, locationsGO.transform);
        locGO.name = e.eventId;
        locGO.transform.position = location.position;
        var handler = locGO.transform.Find("Event").GetComponent<RandomEventHandler>();
        handler.eventId = e.eventId;
    }

    private void SetupProjectLocation(Project project, Location location) {
        GameObject locGO = Instantiate(projectLocationPrefab, locationsGO.transform);
        locGO.name = project.title;
        locGO.transform.position = location.position;
        var handler = locGO.transform.Find("Project").GetComponent<ProjectHandler>();
        handler.projectId = project.projectId;
    }

    private void SetupDailyReportUI(List<string> descriptions) {
        // 先清空
        var content = DailyReportUI.GetComponent<ScrollRect>().content;
        var textList = content.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var t in textList) {
            Destroy(t.gameObject);
        }
        // 再生成新的text
        foreach (string t in descriptions) {
            GameObject GO = Instantiate(dailyResultItemPrefab, content);
            GO.GetComponent<TextMeshProUGUI>().text = t;
        }
        DailyReportConfirmButton.gameObject.SetActive(true);
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

    public void SetPropertyCurrentValue(string propertyName, int value) {
        baseGameProperties.Find(property => property.propertyName == propertyName).currentValue = value;
    }

    public void SetPropertyMaxValue(string propertyName, int value) {
        baseGameProperties.Find(property => property.propertyName == propertyName).maxValue = value;
    }
    
    private void UpdateMainUI() {
        UpdateActionPointUI();
        UpdateProfessionalityUI();
        UpdateGenerosityUI();
        UpdateHandCardUI();
    }

    private void UpdateActionPointUI() {
        int currentActionPoint = GetPropertyCurrentValue("ActionPoints");
        int maxActionPoint = GetPropertyMaxValue("ActionPoints");
        ActionPointUI.transform.Find("ActionPointText").GetComponent<TextMeshProUGUI>().text = $"<size=130%><b>{currentActionPoint}</b></size>/{maxActionPoint}";
    }

    private void UpdateProfessionalityUI() {
        int currentProfessionality = GetPropertyCurrentValue("Professionality");
        int maxProfessionality = GetPropertyMaxValue("Professionality");
        ProfessionalityUI.transform.Find("AttrText").GetComponent<TextMeshProUGUI>().text = $"{currentProfessionality}";
    }

    private void UpdateGenerosityUI() {
        int currentGenerosity = GetPropertyCurrentValue("Generosity");
        int maxGenerosity = GetPropertyMaxValue("Generosity");
        GenerosityUI.transform.Find("AttrText").GetComponent<TextMeshProUGUI>().text = $"{currentGenerosity}";
    }

    public void UpdateHandCardUI() {
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
        // count each kind of card
        var handCardsGroup = HandCards
                                .GroupBy(s => s)
                                .Select(g => new { CardId = g.Key, Count = g.Count () })
                                .ToList();
        foreach (var item in handCardsGroup) {
            SetupHandCardUIbyIdAndCnt(item.CardId, item.Count, content);
        }      
    }

    private void SetupHandCardUIbyIdAndCnt(string cardId, int count, Transform content) {
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

            // card count ui
            var numText = cardGO.transform.Find("NumberOfCards").GetComponent<TextMeshProUGUI>();
            if (count == 1) {
                numText.gameObject.SetActive(false);
            } else {
                numText.text = $"x{count}";
            }
            
            // Set card data
            CardDataHolder cardDataHolder = cardGO.GetComponent<CardDataHolder>();
            cardDataHolder.cardData = cardData;

            // Set card button
            Button cardButton = cardGO.GetComponent<Button>();
            cardButton.onClick.RemoveAllListeners();
            if (!isOnProjectUI) {
                cardButton.onClick.AddListener(displayCardDetail);
            } else {
                cardButton.onClick.AddListener(placeCard);
            }
        }
    }

    private void displayCardDetail() {
        Debug.Log("display card detail");
    }

    private void placeCard() {
        Debug.Log("place card");
        // Get the clicked card data
        CardDataHolder cardDataHolder = EventSystem.current.currentSelectedGameObject.GetComponent<CardDataHolder>();
        if (cardDataHolder == null || cardDataHolder.cardData == null) {
            Debug.Log("cardDataHolder is null or cardData is null");
            return;
        }

        // Remove card from hand
        string cardId = cardDataHolder.cardData.cardId;
        if (HandCards.Contains(cardId)) {
            HandCards.Remove(cardId);
            
            // Find first empty card slot
            Transform cardSlots = ProjectUI.transform.Find("CardSlots");
            if (cardSlots != null) {
                bool hasEmptySlot = false;
                foreach (Transform slot in cardSlots) {
                    if (slot.GetComponent<CardDataHolder>().cardData == null && slot.gameObject.activeSelf) {
                        slot.GetComponent<CardDataHolder>().cardData = cardDataHolder.cardData;
                        currentPlacedCards.Add(cardDataHolder.cardData);
                        // Update hand card UI
                        hasEmptySlot = true;
                        break;
                    } 
                }
                if (!hasEmptySlot) {
                    // TODO: [bug] 这个提示框弹出之后，如果继续点卡牌会报错，后面再DEBUG下
                    Debug.Log("No more empty card slots!");
                    PromptUI.ShowOkPrompt("No more empty card slots!", () => {
                        Debug.Log("After show prompt");
                        return;
                    });
                    return;
                }
                // 每次放置卡牌后，重新把所有放置的卡牌结算一遍
                ProjectUI.ResetDicesAndCardSlots();
                ExecuteCardEffects();
                ProjectUI.UpdateCardSlotUI();
                ProjectUI.UpdateDicesUI();
                UpdateHandCardUI();

            }
        }
    }

    public void ExecuteCardEffects() {
        foreach (Card card in currentPlacedCards) {
            foreach (EffectData effect in card.cardEffects) {
                EffectExecutor.ExecuteEffect(effect.effectCode);
            }
        }
    }

    public int[] RollDices() {
        int[] dices = new int[GetPropertyCurrentValue("HaveDices")];
        for (int i = 0; i < dices.Length; i++) {
            dices[i] = Random.Range(0, 2);
        }
        Debug.Log("RollDices result: " + string.Join(", ", dices));
        return dices;
    }

    public int GetProjectResultIndex(int[] dices) {
        // TODO: 根据骰子结果获取项目结果
        int sum = 0;
        foreach (int dice in dices) {
            sum += dice;
        }
        int needDices = GetPropertyCurrentValue("NeedDices");
        if (sum >= needDices) {
            Debug.Log("NeedDices: " + needDices + " sum: " + sum + " ProjectResultIndex: 1");
            return 1; // 成功
        } else {
            Debug.Log("NeedDices: " + needDices + " sum: " + sum + " ProjectResultIndex: 0");
            return 0; // 失败
        }
    }
}