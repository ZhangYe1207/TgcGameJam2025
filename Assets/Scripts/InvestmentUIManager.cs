using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvestmentUIManager : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public Text amountText;
    public Text successRateText;
    public Image projectImage;
    public Button addButton;
    public Button subButton;
    public Button confirmButton;

    public int investmentStep = 1000;

    public ProjectHandler projectHandler;
    private int currentInvestment = 0;
    private int currentReputation = 0;
    private int minInvestment = 0;
    private int maxInvestment = 0;

    // Start is called before the first frame update
    void Start()
    {   
        addButton.onClick.AddListener(AddInvestment);
        subButton.onClick.AddListener(SubInvestment);
        confirmButton.onClick.AddListener(ConfirmInvestment);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProjectAndInit(ProjectHandler projectHandler) {
        this.projectHandler = projectHandler;
        Init();
    }

    private void Init() {
        if (titleText == null) {
            titleText = transform.Find("Title").GetComponent<Text>();
        }
        if (descriptionText == null) {
            descriptionText = transform.Find("Description").GetComponent<Text>();
        }
        if (amountText == null) {
            amountText = transform.Find("Amount/AmountText").GetComponent<Text>();
        }
        if (successRateText == null) {
            successRateText = transform.Find("SuccessRate").GetComponent<Text>();
        }
        if (projectImage == null) {
            projectImage = transform.Find("ProjectImage").GetComponent<Image>();
        }
        if (addButton == null) {
            addButton = transform.Find("AddButton").GetComponent<Button>();
        }
        if (subButton == null) {
            subButton = transform.Find("SubButton").GetComponent<Button>();
        }
        if (confirmButton == null) {
            confirmButton = transform.Find("ConfirmButton").GetComponent<Button>();
        }
        titleText.text = projectHandler.projectData.title;
        descriptionText.text = projectHandler.projectData.description;
        currentInvestment = projectHandler.projectData.minInvestment;
        currentReputation = PlayerManager.Instance.playerData.reputation;
        maxInvestment = PlayerManager.Instance.playerData.money;
        minInvestment = projectHandler.projectData.minInvestment;
        projectImage.sprite = projectHandler.projectData.projectImage;
        UpdateUI();
    }

    private void UpdateUI() {
        amountText.text = $"{currentInvestment}";
        float successRate = projectHandler.projectData.CalculateShowSuccessRate(currentInvestment, currentReputation);
        successRateText.text = $"Current Success Rate: {successRate * 100}%";
    }

    private void AddInvestment() {
        currentInvestment += investmentStep;
        if (currentInvestment > maxInvestment) {
            currentInvestment = maxInvestment;
        }
        UpdateUI();
    }

    private void SubInvestment() {
        currentInvestment -= investmentStep;
        if (currentInvestment < minInvestment   ) {
            currentInvestment = minInvestment;
        }
        UpdateUI();
    }

    private void ConfirmInvestment() {
        // TODO: 确认投资
    }
    
}
