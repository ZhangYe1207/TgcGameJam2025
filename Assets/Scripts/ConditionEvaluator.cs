using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConditionEvaluator : MonoBehaviour
{
    private Dictionary<string, GameProperty> baseGameProperties;
    private Dictionary<string, FieldInfo> listGameProperties;
    private static ConditionEvaluator instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);        
    }

    public void Start() {
        InitializeGameProperties();
    }

    private void InitializeGameProperties()
    {
        baseGameProperties = new Dictionary<string, GameProperty>();
        listGameProperties = new Dictionary<string, FieldInfo>();
        foreach (var property in GameManager.Instance.baseGameProperties) {
            baseGameProperties[property.propertyName] = property;
        }
        listGameProperties["HandCards"] = typeof(GameManager).GetField("HandCards");
        listGameProperties["EventsFinished"] = typeof(GameManager).GetField("EventsFinished");
        listGameProperties["ProjectFinished"] = typeof(GameManager).GetField("ProjectFinished");
        listGameProperties["Friends"] = typeof(GameManager).GetField("Friends");
    }

    public static bool EvaluateCondition(string conditionCode)
    {
        return instance.InternalEvaluateCondition(conditionCode);
    }

    private bool InternalEvaluateCondition(string conditionCode)
    {
        // Check if it's a list operation (contains "has")
        if (conditionCode.Contains(":has:"))
        {
            return EvaluateListCondition(conditionCode);
        }
        
        return EvaluateNumberCondition(conditionCode);
    }

    private bool EvaluateNumberCondition(string conditionCode)
    {
        string[] parts = conditionCode.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
        {
            Debug.LogError($"Invalid condition format: {conditionCode}");
            return false;
        }

        string propertyName = parts[0].Trim();
        string operation = parts[1].Trim();
        string valueString = parts[2].Trim();

        if (baseGameProperties.TryGetValue(propertyName, out GameProperty property))
        {
            try
            {
                int currentValue = property.currentValue;
                int compareValue = int.Parse(valueString);

                return EvaluateComparison(currentValue, operation, compareValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error evaluating condition: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError($"Property not found: {propertyName}");
            return false;
        }
    }

    private bool EvaluateListCondition(string conditionCode)
    {
        string[] parts = conditionCode.Split(new[] { ":has:" }, StringSplitOptions.None);
        if (parts.Length != 2)
        {
            Debug.LogError($"Invalid list condition format: {conditionCode}");
            return false;
        }

        string listName = parts[0].Trim();
        string valueToCheck = parts[1].Trim();

        if (listGameProperties.TryGetValue(listName, out FieldInfo property))
        {
            try
            {
                IList list = property.GetValue(GameManager.Instance) as IList;
                if (list == null)
                {
                    Debug.LogError($"Property {listName} is not a valid list");
                    return false;
                }

                // Check if the list contains the value
                foreach (var item in list)
                {
                    string itemId = "";
                    if (listName == "HandCards") {
                        itemId = (item as Card).cardId;
                    } else {
                        itemId = item.ToString();
                    }
                    if (itemId == valueToCheck)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error evaluating list condition: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError($"List property not found: {listName}");
            return false;
        }
    }

    private bool EvaluateComparison(int currentValue, string operation, int compareValue)
    {
        switch (operation)
        {
            case ">=":
                return currentValue >= compareValue;
            case ">":
                return currentValue > compareValue;
            case "<=":
                return currentValue <= compareValue;
            case "<":
                return currentValue < compareValue;
            case "=":
                return currentValue == compareValue;
            default:
                Debug.LogError($"Unsupported comparison operator: {operation}");
                return false;
        }
    }
}    