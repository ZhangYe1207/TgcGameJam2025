using UnityEngine;

public class InvestmentEvent : Event
{
    [Header("Investment Properties")]
    public float investmentCost = 1000f;
    public float returnRate = 0.1f; // 10% return rate
    public float currentInvestment = 0f;
    public float maxInvestment = 10000f;
    
    protected override void Start()
    {
        base.Start();
        currentInvestment = 0f;
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
    
    public float GetReturn()
    {
        return currentInvestment * returnRate;
    }
    
    public float WithdrawInvestment()
    {
        float returnAmount = currentInvestment;
        currentInvestment = 0f;
        return returnAmount;
    }
} 