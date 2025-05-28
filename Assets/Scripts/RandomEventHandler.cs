using UnityEngine;
using UnityEngine.UI;

public class RandomEventHandler : MonoBehaviour
{
    public string eventId;
    public RandomEvent eventData;
    public float eventTriggerRadius = 5f;

    private ResourceEventUIManager resourceEventUI;
    private PromptUIManager promptUI;
    private GameObject locationGO;
    private PlayerController playerController;
    private bool waitingForConfirm = false;
    private bool isFinished = false;
    
    public void Start() {
        eventData = DatabaseManager.Instance.eventDatabase.GetEventByID(eventId);
        if (eventData == null) {
            Debug.LogError("Event not found: " + eventId);
        }
        locationGO = gameObject.transform.parent.gameObject;
        resourceEventUI = GameObject.Find("Canvas").transform.Find("ResourceEventPanel").GetComponent<ResourceEventUIManager>();
        Debug.Log("resourceEventUI: " + resourceEventUI);
        promptUI = GameObject.Find("Canvas").transform.Find("PromptUI").GetComponent<PromptUIManager>();
        Debug.Log("promptUI: " + promptUI);
        playerController = GameManager.Instance.playerGO.GetComponent<PlayerController>();
    }

    public void Update() {
        if (promptUI.gameObject.activeInHierarchy) {
            return; // 如果PromptUI显示中，等待PromptUI来处理
        }
        if (waitingForConfirm) {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) {
                ConfirmEvent();
                return;
            }
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
        if (eventData.eventType == EventType.Resource) {
            resourceEventUI.SetRandomEventHandler(this);
            resourceEventUI.ShowEvent(eventData);
        }
    }

    private bool CheckPrerequisites() {
        foreach (EventPrerequisite prerequisite in eventData.prerequisites) {
            foreach (ConditionData condition in prerequisite.conditions) {
                if (!ConditionEvaluator.EvaluateCondition(condition.conditionCode)) {
                    waitingForConfirm = true;
                    playerController.isLocked = true;
                    Debug.Log($"事件{eventId}的前置条件{condition.conditionCode}不满足，无法触发事件");
                    promptUI.ShowOkPrompt(
                        condition.failedMessage,
                        () => {
                            waitingForConfirm = false;
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
        waitingForConfirm = false;
        resourceEventUI.HideEvent();
        Debug.Log($"事件{eventId}已确认, 退出UI");
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
        isFinished = true;
        return;
    }

    private bool IsPlayerNearby() {
        Vector3 locationPosition = locationGO.transform.position;
        locationPosition.y = 0;
        Vector3 playerPosition = GameManager.Instance.playerGO.transform.position;
        playerPosition.y = 0;
        return Vector3.Distance(locationPosition, playerPosition) < eventTriggerRadius;
    }

    private bool MouseHovering() {
        // TODO: 鼠标悬停事件处理
        return false;
    }
    
    public void HandleResourceEvent(EventResult result) {
        // 按顺序执行效果
        foreach (EffectData effect in result.eventEffects) {
            EffectExecutor.ExecuteEffect(effect.effectCode);
        }
        GameManager.Instance.OnGameDataChanged();
    }
}
