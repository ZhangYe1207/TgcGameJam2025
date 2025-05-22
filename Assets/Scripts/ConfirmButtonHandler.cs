using UnityEngine;
using UnityEngine.UI;

public class ConfirmButtonHandler : MonoBehaviour
{
    public Button confirmButton;  // 确认按钮
    public GameObject uiPanel;    // UI面板对象
    public PlayerController player; // 玩家控制器引用

    void Start()
    {
        // 注册按钮点击事件
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    void OnConfirmButtonClick()
    {
        // 关闭UI面板
        uiPanel.SetActive(false);
        
        // 设置玩家状态
        if (player != null)
        {
            player.isLocked = false;
        }
    }
}