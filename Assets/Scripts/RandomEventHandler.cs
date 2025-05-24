using UnityEngine;
using UnityEngine.UI;

public class RandomEventHandler : MonoBehaviour
{
    public string eventId;
    public RandomEvent eventData;
    public float eventTriggerRadius = 5f;

    private GameObject eventResultUI;
    private GameObject locationGO;
    private bool waitingForConfirm = false;
    private bool isFinished = false;
    
    public void Start() {
        eventData = GameManager.Instance.eventDatabase.GetEventByID(eventId);
        if (eventData == null) {
            Debug.LogError("Event not found: " + eventId);
        }
        locationGO = gameObject.transform.parent.gameObject;
        eventResultUI = GameObject.Find("Canvas").transform.Find("ResourceEventPanel").gameObject;
        Debug.Log("eventResultUI: " + eventResultUI);
    }

    public void Update() {
        if (waitingForConfirm) {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) {
                waitingForConfirm = false;
                eventResultUI.SetActive(false);
                Debug.Log($"事件{eventId}已确认, 退出UI");
                PlayerManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
                isFinished = true;
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
            if (Input.GetKeyDown(KeyCode.E)) {
                HandleEvent();
            }
        }
    }

    private bool IsPlayerNearby() {
        Vector3 locationPosition = locationGO.transform.position;
        locationPosition.y = 0;
        Vector3 playerPosition = PlayerManager.Instance.playerGO.transform.position;
        playerPosition.y = 0;
        return Vector3.Distance(locationPosition, playerPosition) < eventTriggerRadius;
    }

    private bool MouseHovering() {
        // TODO: 鼠标悬停事件处理
        return false;
    }

    private void HandleEvent() {
        Debug.Log("Event triggered: " + eventId);
        if (eventData.eventType == EventType.Resource) {
            HandleResourceEvent();
        } else if (eventData.eventType == EventType.Project) {
            HandleProjectEvent();
        }
    }
    
    private void HandleResourceEvent() {
        // TODO: 资源事件处理
        // 1. 结果结算, 随机选择一个结果
        EventResult result = eventData.results[Random.Range(0, eventData.results.Length)];
        // Debug.Log($"资源事件{eventId}结果: {result.description}");
        // 2. 显示结算UI
        ShowResourceEventResultUI(result);
        // 3. 更新玩家数据
        PlayerManager.Instance.AddEventFinished(eventId);
        PlayerManager.Instance.AddMoney(result.moneyChange);
        PlayerManager.Instance.AddReputation(result.reputationChange);
        // 4. 更新卡牌
        foreach (int cardId in result.addCardIds) {
            PlayerManager.Instance.AddCardById(cardId);
        }
        foreach (int cardId in result.removeCardIds) {
            PlayerManager.Instance.RemoveCardById(cardId);
        }
    }

    private void ShowResourceEventResultUI(EventResult result) {
        eventResultUI.transform.Find("Title").GetComponent<Text>().text = eventData.title;
        eventResultUI.transform.Find("Description").GetComponent<Text>().text = eventData.description;
        eventResultUI.transform.Find("EventImage").GetComponent<Image>().sprite = eventData.eventImage;
        eventResultUI.transform.Find("Result").GetComponent<Text>().text = result.description;
        eventResultUI.SetActive(true);
        waitingForConfirm = true;
        PlayerController playerController = PlayerManager.Instance.playerGO.GetComponent<PlayerController>();
        playerController.isLocked = true;
    }

    private void HandleProjectEvent() {
        // TODO: 项目事件处理
    }
}
