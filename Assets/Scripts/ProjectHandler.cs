using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectHandler : MonoBehaviour
{
    public string projectId;
    public Project projectData;
    public float eventTriggerRadius = 5f;

    private GameObject projectInvestmentUI;
    private GameObject locationGO;
    private bool waitingForConfirm = false;
    private bool isFinished = false;

    void Start()
    {
        projectData = DatabaseManager.Instance.projectDatabase.GetProjectByID(projectId);
        if (projectData == null) {
            Debug.LogError("Project not found: " + projectId);
        }
        locationGO = gameObject.transform.parent.gameObject;
        // projectInvestmentUI = GameObject.Find("Canvas").transform.Find("ProjectInvestmentPanel").gameObject;
        // Debug.Log("projectInvestmentUI: " + projectInvestmentUI);
    }

    public void Update() {
        if (waitingForConfirm) {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) {
                ConfirmInvestment();
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
                HandleProject();
            }
        }
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

    private void HandleProject() {
        // TODO: 项目投资UI界面&交互逻辑
        // projectInvestmentUI.GetComponent<InvestmentUIManager>().SetProjectAndInit(this);
        // projectInvestmentUI.SetActive(true);
        waitingForConfirm = true;
        PlayerController playerController = GameManager.Instance.playerGO.GetComponent<PlayerController>();
        playerController.isLocked = true;
    }
    
    public void ConfirmInvestment() {
        // TODO: 消耗行动点
        // PlayerManager.Instance.UseActionPoint();
        
        waitingForConfirm = false;
        // projectInvestmentUI.SetActive(false);
        Debug.Log($"项目投资{projectId}已确认, 退出UI");
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
        isFinished = true;
        return;
    }
}
