using UnityEngine;
using TMPro;
using DialogueEditor;
using UnityEngine.UI;

public class NPCHandler : MonoBehaviour {
    public string npcName;
    public NPC npcData;
    public TextMeshProUGUI EText;
    public float eventTriggerRadius = 5f;
    public bool isFinished = false;
    public Image locationSign;

    private Color maskColor = new Color(80f/255, 80f/255, 80f/255, 128f/255);

    private GameObject locationGO;

    public void Start() {
        npcData = DatabaseManager.Instance.npcDatabase.GetNPCByName(npcName);
        locationGO = gameObject.transform.parent.gameObject;
    }

    public void Update() {
        if (npcData == null) {
            npcData = DatabaseManager.Instance.npcDatabase.GetNPCByName(npcName);
            if (npcData == null) {
                Debug.LogWarning("NPC not found: " + npcName);
            }
        }
        if (IsPlayerNearby()) {
            // TODO: 显示预览UI
        }
        if (isFinished) {
            return;
        }
        if (IsPlayerNearby()) {
            if (Input.GetKeyDown(KeyCode.E)) {
                NPCConversation conv = GameObject.Find(npcData.npcConversationName).GetComponent<NPCConversation>();
                if (conv == null) {
                    Debug.LogWarning("NPC Conversation not found: " + npcName);
                }
                ConversationManager.Instance.StartConversation(conv);
                GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = true;
                ConversationManager.OnConversationEnded += ConversationEnd;
            }
        }
    }

    private bool IsPlayerNearby() {
        Vector3 locationPosition = locationGO.transform.position;
        locationPosition.y = 0;
        Vector3 playerPosition = GameManager.Instance.playerGO.transform.position;
        playerPosition.y = 0;
        bool res = Vector3.Distance(locationPosition, playerPosition) < eventTriggerRadius;
        EText.gameObject.SetActive(res && !isFinished);
        return res;
    }

    public void ConversationEnd() {
        int idx = ConversationManager.Instance.GetInt("ResultIndex");
        Debug.Log($"{npcData.npcConversationName} end, result index: {idx}");
        NPCResult result = npcData.results[idx];
        foreach (EffectData effect in result.immediateEffects) {
            EffectExecutor.ExecuteEffect(effect.effectCode);
        }
        foreach (DelayedEffectData effect in result.delayedEffects) {
            GameManager.Instance.DelayedEffects.Add(effect);
        }
        GameManager.Instance.OnGameDataChanged();
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
        locationSign.color = maskColor;
        isFinished = true;
        ConversationManager.OnConversationEnded -= ConversationEnd;
    }
}