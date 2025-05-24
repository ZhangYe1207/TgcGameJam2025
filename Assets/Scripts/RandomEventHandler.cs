using UnityEngine;

public class RandomEventHandler : MonoBehaviour
{
    public string eventId;
    public RandomEvent eventData;
    public GameObject locationGO;
    public float eventTriggerRadius = 5f;
    
    public void Start() {
        eventData = GameManager.Instance.eventDatabase.GetEventByID(eventId);
        if (eventData == null) {
            Debug.LogError("Event not found: " + eventId);
        }
        locationGO = gameObject.transform.parent.gameObject;
    }

    public void Update() {
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

    private void HandleEvent() {
        Debug.Log("Event triggered: " + eventId);
        if (eventData.eventType == RandomEventType.Resource) {
            HandleResourceEvent();
        } else if (eventData.eventType == RandomEventType.Project) {
            HandleProjectEvent();
        }
    }
    
    private void HandleResourceEvent() {
        // 资源事件处理
    }

    private void HandleProjectEvent() {
        // 项目事件处理
    }

}
