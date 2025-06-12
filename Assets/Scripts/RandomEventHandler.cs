using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomEventHandler : MonoBehaviour
{
    public string eventId;
    public RandomEvent eventData;
    public float eventTriggerRadius = 2f;

    private ResourceEventUIManager resourceEventUI;
    private PromptUIManager promptUI;
    private GameObject locationGO;
    private PlayerController playerController;
    public bool isFinished = false;
    public Image locationSign;
    public TextMeshProUGUI EText;
    private Color maskColor = new Color(80f/255, 80f/255, 80f/255, 128f/255);

    
    public void Start() {
        eventData = DatabaseManager.Instance.eventDatabase.GetEventByID(eventId);
        if (eventData == null) {
            Debug.LogWarning("Event not found: " + eventId);
        }
        locationGO = gameObject.transform.parent.gameObject;
        resourceEventUI = GameObject.Find("Canvas").transform.Find("ResourceEventPanel").GetComponent<ResourceEventUIManager>();
        promptUI = GameObject.Find("Canvas").transform.Find("PromptUI").GetComponent<PromptUIManager>();
        playerController = GameManager.Instance.playerGO.GetComponent<PlayerController>();
    }

    public void Update() {
        if (eventData == null) {
            eventData = DatabaseManager.Instance.eventDatabase.GetEventByID(eventId);
            if (eventData == null) {
                Debug.LogWarning("Event not found: " + eventId);
            }
        }
        if (promptUI.gameObject.activeInHierarchy) {
            return; // 如果PromptUI显示中，等待PromptUI来处理
        }
        if (IsPlayerNearby() || MouseHovering()) {
            // TODO: 显示预览UI
        }
        if (isFinished) {
            return;
        }
        if (IsPlayerNearby()) {
            if (Input.GetKeyDown(KeyCode.E) && CheckPrerequisites()) {
                ShowEventUI();
            }
        }
    }

    private void ShowEventUI() {
        if (eventData.eventType == RandomEventType.Resource) {
            resourceEventUI.SetRandomEventHandler(this);
            resourceEventUI.ShowEvent(eventData);
        }
    }

    private bool CheckPrerequisites() {
        foreach (EventPrerequisite prerequisite in eventData.prerequisites) {
            foreach (ConditionData condition in prerequisite.conditions) {
                if (!ConditionEvaluator.EvaluateCondition(condition.conditionCode)) {
                    playerController.isLocked = true;
                    Debug.Log($"事件{eventId}的前置条件{condition.conditionCode}不满足，无法触发事件");
                    promptUI.ShowOkPrompt(
                        condition.failedMessage,
                        () => {
                            playerController.isLocked = false;
                            return;
                        }
                    );
                    return false;
                }
            }
        }
        return true;
    }

    public void ConfirmEvent() {
        resourceEventUI.HideEvent();
        Debug.Log($"事件{eventId}已确认, 退出UI");
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
        isFinished = true;
        locationSign.color = maskColor;
        return;
    }

    private bool IsPlayerNearby() {
        Vector3 locationPosition = locationGO.transform.position;
        locationPosition.y = 0;
        Vector3 playerPosition = GameManager.Instance.playerGO.transform.position;
        playerPosition.y = 0;
        bool res = Vector3.Distance(locationPosition, playerPosition) < eventTriggerRadius;
        EText.gameObject.SetActive(res && !isFinished);;
        return res;
    }

    private bool MouseHovering() {
        // TODO: 鼠标悬停事件处理
        return false;
    }
    
    public void HandleResourceEvent(EventResult result) {
        // 按顺序执行效果
        foreach (EffectData effect in result.immediateEffects) {
            EffectExecutor.ExecuteEffect(effect.effectCode);
        }
        foreach (DelayedEffectData effect in result.delayedEffects) {
            GameManager.Instance.DelayedEffects.Add(effect);
        }
        GameManager.Instance.OnGameDataChanged();
    }
}
