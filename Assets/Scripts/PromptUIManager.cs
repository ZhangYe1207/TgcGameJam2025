using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PromptUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Button okButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI messageText;

    private Action onOkCallback;
    private Action onConfirmCallback;
    private Action onCancelCallback;

    private void Awake()
    {
        // Hide the panel initially
        promptPanel.SetActive(false);

        // Add button listeners
        okButton.onClick.AddListener(OnOkClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    public void ShowOkPrompt(string message, Action onOk = null)
    {
        messageText.text = message;
        onOkCallback = onOk;

        // Show only OK button
        okButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        promptPanel.SetActive(true);
    }

    public void ShowConfirmCancelPrompt(string message, Action onConfirm = null, Action onCancel = null)
    {
        messageText.text = message;
        onConfirmCallback = onConfirm;
        onCancelCallback = onCancel;

        // Show Confirm and Cancel buttons
        okButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);

        promptPanel.SetActive(true);
    }

    private void OnOkClicked()
    {
        Debug.Log("OnOkClicked");
        onOkCallback?.Invoke();
        HidePrompt();
    }

    private void OnConfirmClicked()
    {
        Debug.Log("OnConfirmClicked");
        onConfirmCallback?.Invoke();
        HidePrompt();
    }

    private void OnCancelClicked()
    {
        Debug.Log("OnCancelClicked");
        onCancelCallback?.Invoke();
        HidePrompt();
    }

    private void HidePrompt()
    {
        promptPanel.SetActive(false);
        // Clear callbacks
        onOkCallback = null;
        onConfirmCallback = null;
        onCancelCallback = null;
    }
} 