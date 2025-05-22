using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProjectEvent : Event
{
    [System.Serializable]
    public class HiddenAttribute
    {
        public string attributeName;
        public float value;
        public bool isDiscovered;
        public float discoveryDifficulty;
    }

    [System.Serializable]
    public class ProjectReward
    {
        public string rewardName;
        public float baseAmount;
        public float bonusAmount;
    }

    [Header("Project Properties")]
    public float baseSuccessRate = 0.5f;
    public float baseReturnRate = 0.1f;
    public float currentInvestment = 0f;
    public float maxInvestment = 10000f;
    public List<HiddenAttribute> hiddenAttributes = new List<HiddenAttribute>();
    public List<ProjectReward> rewards = new List<ProjectReward>();
    
    [Header("UI References")]
    public Text successRateText;
    public Text expectedReturnText;
    public Text currentInvestmentText;
    public Text maxInvestmentText;
    public InputField investmentInput;
    public Button investButton;
    public Button withdrawButton;
    public Transform attributesContainer;
    public GameObject attributePrefab;
    
    protected override void Start()
    {
        base.Start();
        InitializeUI();
    }
    
    protected virtual void InitializeUI()
    {
        UpdateUI();
        
        if (investButton != null)
            investButton.onClick.AddListener(OnInvestButtonClicked);
        if (withdrawButton != null)
            withdrawButton.onClick.AddListener(OnWithdrawButtonClicked);
            
        if (attributesContainer != null)
        {
            foreach (var attr in hiddenAttributes)
            {
                CreateAttributeUI(attr);
            }
        }
    }
    
    protected virtual void CreateAttributeUI(HiddenAttribute attribute)
    {
        if (attributePrefab == null) return;
        
        GameObject attrObj = Instantiate(attributePrefab, attributesContainer);
        Text attrText = attrObj.GetComponentInChildren<Text>();
        if (attrText != null)
        {
            if (attribute.isDiscovered)
            {
                attrText.text = $"{attribute.attributeName}: {attribute.value}";
            }
            else
            {
                attrText.text = $"{attribute.attributeName}: ???";
            }
        }
    }
    
    protected virtual void UpdateUI()
    {
        if (successRateText != null)
            successRateText.text = $"Success Rate: {CalculateDisplaySuccessRate() * 100:F1}%";
        if (expectedReturnText != null)
            expectedReturnText.text = $"Expected Return: {CalculateExpectedReturn():F0}";
        if (currentInvestmentText != null)
            currentInvestmentText.text = $"Current Investment: {currentInvestment:F0}";
        if (maxInvestmentText != null)
            maxInvestmentText.text = $"Max Investment: {maxInvestment:F0}";
    }
    
    protected virtual float CalculateDisplaySuccessRate()
    {
        float totalRate = baseSuccessRate;
        
        // Add investment contribution
        totalRate += (currentInvestment / maxInvestment) * 0.3f;
        
        // Add discovered attributes
        foreach (var attr in hiddenAttributes)
        {
            if (attr.isDiscovered)
            {
                totalRate += attr.value;
            }
        }
        
        return Mathf.Clamp01(totalRate);
    }
    
    protected virtual float CalculateActualSuccessRate()
    {
        float totalRate = baseSuccessRate;
        
        // Add investment contribution
        totalRate += (currentInvestment / maxInvestment) * 0.3f;
        
        // Add all attributes (discovered or not)
        foreach (var attr in hiddenAttributes)
        {
            totalRate += attr.value;
        }
        
        return Mathf.Clamp01(totalRate);
    }
    
    protected virtual float CalculateExpectedReturn()
    {
        float totalReturn = 0f;
        foreach (var reward in rewards)
        {
            totalReturn += reward.baseAmount + reward.bonusAmount;
        }
        return totalReturn * CalculateDisplaySuccessRate();
    }
    
    protected virtual void OnInvestButtonClicked()
    {
        if (investmentInput != null && float.TryParse(investmentInput.text, out float amount))
        {
            if (Invest(amount))
            {
                investmentInput.text = "";
                UpdateUI();
            }
        }
    }
    
    protected virtual void OnWithdrawButtonClicked()
    {
        float returnAmount = WithdrawInvestment();
        UpdateUI();
        // You might want to show some feedback here
    }
    
    public bool Invest(float amount)
    {
        if (currentInvestment + amount > maxInvestment)
        {
            return false;
        }
        
        currentInvestment += amount;
        return true;
    }
    
    public float WithdrawInvestment()
    {
        float returnAmount = currentInvestment;
        currentInvestment = 0f;
        return returnAmount;
    }
    
    public bool TryDiscoverAttribute(HiddenAttribute attribute)
    {
        if (attribute.isDiscovered) return false;
        
        float discoveryChance = 1f - (attribute.discoveryDifficulty / 100f);
        if (Random.value <= discoveryChance)
        {
            attribute.isDiscovered = true;
            UpdateUI();
            return true;
        }
        
        return false;
    }
    
    public void CompleteProject()
    {
        float actualSuccessRate = CalculateActualSuccessRate();
        if (Random.value <= actualSuccessRate)
        {
            // Success - give all rewards
            foreach (var reward in rewards)
            {
                float totalReward = reward.baseAmount + reward.bonusAmount;
                // TODO: Add the reward to player's inventory/currency
                Debug.Log($"Project success! Giving reward: {reward.rewardName} x {totalReward}");
            }
        }
        else
        {
            // Failure - you might want to handle this differently
            Debug.Log("Project failed!");
        }
        
        // Reset investment
        currentInvestment = 0f;
        UpdateUI();
    }
    
} 