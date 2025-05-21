using UnityEngine;
using UnityEngine.UI;

public abstract class Event : MonoBehaviour
{
    [Header("Basic Properties")]
    public string eventName;
    public string description;
    public Sprite eventIcon;
    public Sprite backgroundImage;
    
    [Header("UI References")]
    public GameObject eventPanel;
    public GameObject eventPreviewPanel;
    public Text nameText;
    public Text descriptionText;
    public Image iconImage;
    
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    
    protected bool isPlayerNearby = false;
    protected GameObject parentLocation;
    protected GameObject player;
    // 是否显示面板
    protected bool isPanelVisible = false;
    
    protected virtual void Start()
    {
        // Find player transform
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO;
        }
        parentLocation = this.gameObject.transform.parent.gameObject;
    }
    
    protected virtual void Update()
    {
        if (player == null) return;
        // 获取玩家位置
        Transform playerTransform = player.GetComponent<Transform>();

        // Check if player is nearby
        float distance = Vector3.Distance(parentLocation.transform.position, playerTransform.position);
        isPlayerNearby = distance <= interactionDistance;
        Debug.Log("isPlayerNearby: " + isPlayerNearby);
        // Show interaction prompt when nearby
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            player.GetComponent<PlayerController>().isLocked = true;
            Debug.Log("Player locked");
            ToggleEventPanel();
        }
    }
    
    protected virtual void ToggleEventPanel()
    {
        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
            
            if (isPanelVisible)
            {
                OnPanelOpened();
            }
            else
            {
                OnPanelClosed();
            }
        }
    }
    
    protected virtual void OnPanelOpened()
    {
        // Override this in derived classes to handle panel opening
    }
    
    protected virtual void OnPanelClosed()
    {
        // Override this in derived classes to handle panel closing
    }
} 