using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResourceEvent : Event
{
    [System.Serializable]
    public class ResourceReward
    {
        public string rewardName;
        public float baseAmount;
        public float randomBonusMin;
        public float randomBonusMax;
        public float probability = 1f;
    }

    [System.Serializable]
    public class ResourceOption
    {
        public string optionName;
        public string description;
        public List<ResourceReward> rewards;
        public float successRate = 1f;
    }

    [Header("Resource Properties")]
    public List<ResourceReward> fixedRewards = new List<ResourceReward>();
    public List<ResourceOption> options = new List<ResourceOption>();
    
    [Header("UI References")]
    public Transform rewardsContainer;
    public Transform optionsContainer;
    public GameObject rewardPrefab;
    public GameObject optionPrefab;
    
    protected override void Start()
    {
        base.Start();
        InitializeUI();
    }
    
    /// <summary>
    /// 设置事件面板
    /// </summary>
    protected override void SetupEventPanel()
    {
        Debug.Log("SetupEventPanel");
        Text eventNameText = eventPanel.transform.Find("Name").GetComponent<Text>();
        Text descriptionText = eventPanel.transform.Find("Description").GetComponent<Text>();
        Image backgroundImage = eventPanel.transform.Find("BackgroundImage").GetComponent<Image>();
        eventNameText.text = eventName;
        descriptionText.text = description;
        backgroundImage.sprite = backgroundImageSprite;
        Debug.Log("SetupEventPanel done");
    }
    
    protected virtual void InitializeUI()
    {
        if (rewardsContainer != null)
        {
            foreach (var reward in fixedRewards)
            {
                CreateRewardUI(reward);
            }
        }
        
        if (optionsContainer != null)
        {
            foreach (var option in options)
            {
                CreateOptionUI(option);
            }
        }
    }
    
    protected virtual void CreateRewardUI(ResourceReward reward)
    {
        if (rewardPrefab == null) return;
        
        GameObject rewardObj = Instantiate(rewardPrefab, rewardsContainer);
        Text rewardText = rewardObj.GetComponentInChildren<Text>();
        if (rewardText != null)
        {
            string bonusText = reward.randomBonusMax > 0 ? 
                $" (+{reward.randomBonusMin}-{reward.randomBonusMax})" : "";
            rewardText.text = $"{reward.rewardName}: {reward.baseAmount}{bonusText}";
        }
    }
    
    protected virtual void CreateOptionUI(ResourceOption option)
    {
        if (optionPrefab == null) return;
        
        GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
        Text optionText = optionObj.GetComponentInChildren<Text>();
        if (optionText != null)
        {
            optionText.text = $"{option.optionName} ({option.successRate * 100}% success)";
        }
        
        Button optionButton = optionObj.GetComponent<Button>();
        if (optionButton != null)
        {
            optionButton.onClick.AddListener(() => OnOptionSelected(option));
        }
    }
    
    protected virtual void OnOptionSelected(ResourceOption option)
    {
        if (Random.value <= option.successRate)
        {
            // Success - give all rewards
            foreach (var reward in option.rewards)
            {
                GiveReward(reward);
            }
        }
        // You might want to show some feedback here
    }
    
    protected virtual void GiveReward(ResourceReward reward)
    {
        if (Random.value > reward.probability) return;
        
        float randomBonus = 0f;
        if (reward.randomBonusMax > 0)
        {
            randomBonus = Random.Range(reward.randomBonusMin, reward.randomBonusMax);
        }
        
        float totalReward = reward.baseAmount + randomBonus;
        // TODO: Add the reward to player's inventory/currency
        Debug.Log($"Giving reward: {reward.rewardName} x {totalReward}");
    }
    
} 