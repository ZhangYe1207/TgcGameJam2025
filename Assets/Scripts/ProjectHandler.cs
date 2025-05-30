using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectHandler : MonoBehaviour
{
    public string projectId;
    public Project projectData;
    public float eventTriggerRadius = 5f;

    private ProjectUIManager projectUI;
    private GameObject locationGO;
    private bool isFinished = false;

    void Start()
    {
        projectData = DatabaseManager.Instance.projectDatabase.GetProjectByID(projectId);
        if (projectData == null) {
            Debug.LogError("Project not found: " + projectId);
        }
        locationGO = gameObject.transform.parent.gameObject;
        projectUI = GameObject.Find("Canvas").transform.Find("ProjectUI").GetComponent<ProjectUIManager>();
    }

    public void Update() {
        if (projectUI.gameObject.activeInHierarchy) {
            // 交给ProjectUI来处理
            return;
        }
        if (IsPlayerNearby() || MouseHovering()) {
            // TODO: 显示预览UI
        }
        if (isFinished) {
            return;
        }
        if (IsPlayerNearby()) {
            if (Input.GetKeyDown(KeyCode.E)) {
                ShowProjectUI();
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
        PlayerController playerController = GameManager.Instance.playerGO.GetComponent<PlayerController>();
        playerController.isLocked = true;
    }

    private void ShowProjectUI() {
        projectUI.SetProjectHandler(this);
        projectUI.ShowProject();
    }
    
    public void ConfirmInvestment() {
        Debug.Log($"项目投资{projectId}已确认, 退出UI");
        GameManager.Instance.playerGO.GetComponent<PlayerController>().isLocked = false;
        isFinished = true;
        return;
    }
}
