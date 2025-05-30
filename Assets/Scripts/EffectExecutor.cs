using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EffectExecutor : MonoBehaviour
{
    private Dictionary<string, GameProperty> baseGameProperties;
    private Dictionary<string, FieldInfo> listGameProperties;
    private string[] supportedOperations = new string[] { "+=", "-=", "*=", "/=", "=" };
    private string[] supportedListOperations = new string[] { "add", "remove", "clear" };
    private string[] specialEffectCodes = new string[] { "xxxx" };
    private static EffectExecutor instance;

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
        listGameProperties["DelayedEffects0"] = typeof(GameManager).GetField("DelayedEffects0");
        listGameProperties["DelayedEffects1"] = typeof(GameManager).GetField("DelayedEffects1");
        listGameProperties["DelayedEffects2"] = typeof(GameManager).GetField("DelayedEffects2");
        listGameProperties["DelayedEffects3"] = typeof(GameManager).GetField("DelayedEffects3");
        // foreach (var property in baseGameProperties) {
        //     Debug.Log(property.Key + ": " + property.Value);
        // }
        // foreach (var property in listGameProperties) {
        //     Debug.Log(property.Key + ": " + string.Join(", ", property.Value));
        // }
    }

    public static void ExecuteEffect(string effectCode)
    {
        instance.InternalExecuteEffect(effectCode);
    }


    private void InternalExecuteEffect(string effectCode)
    {
        // 解析List操作的特殊格式: "listName:add:value"
        if (effectCode.Contains(":"))
        {
            ExecuteListOperation(effectCode);
            return;
        }
        
        ExecuteNumberOperation(effectCode);
    }

    private void ExecuteNumberOperation(string effectCode)
    {
        // 基础类型操作解析
        string[] parts = effectCode.Split(supportedOperations, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            Debug.LogError($"无效的效果代码: {effectCode}");
            return;
        }

        try {
            string propertyName = parts[0].Trim();
            string valueString = parts[1].Trim();
            int value = int.Parse(valueString);
            if (baseGameProperties.TryGetValue(propertyName, out GameProperty property)) {
                int oldValue = property.currentValue;
                foreach (var operation in supportedOperations) {
                    if (effectCode.Contains(operation)) {
                        ApplyNumberOperation(property, value, operation);
                        Debug.Log($"执行效果 {effectCode} 成功，{propertyName}: {oldValue} -> {property.currentValue}");
                        return;
                    }
                }
            }
            else {
                Debug.LogError($"未找到属性: {propertyName}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"执行效果时出错: {e.Message}");
        }
    }

    private int HandleOutOfRange(GameProperty property, int value, string operation)
    {
        switch (property.outOfRangeHandlePolicy) {
            case GamePropertyOutOfRangeHandlePolicy.Clamp:
                return Mathf.Clamp(value, property.minValue, property.maxValue);
            case GamePropertyOutOfRangeHandlePolicy.Error:
                Debug.LogError($"属性 {property.propertyName} 超出范围: {value}");
                return value;
            default:
                Debug.LogError($"不支持的超出范围处理策略: {property.outOfRangeHandlePolicy}");
                return value;
        }
    }
    
    private void ApplyNumberOperation(GameProperty property, int value, string operation)
    {
        int currentValue = property.currentValue;
        int newValue;

        switch (operation)
        {
            case "+=":
                newValue = currentValue + value;
                if (newValue > property.maxValue || newValue < property.minValue) {
                    newValue = HandleOutOfRange(property, newValue, operation);
                }
                property.currentValue = newValue;
                break;
            case "-=":
                newValue = currentValue - value;
                if (newValue > property.maxValue || newValue < property.minValue) {
                    newValue = HandleOutOfRange(property, newValue, operation);
                }
                property.currentValue = newValue;
                break;
            case "*=":
                newValue = currentValue * value;
                if (newValue > property.maxValue || newValue < property.minValue) {
                    newValue = HandleOutOfRange(property, newValue, operation);
                }
                property.currentValue = newValue;
                break;
            case "/=":
                newValue = currentValue / value;
                if (newValue > property.maxValue || newValue < property.minValue) {
                    newValue = HandleOutOfRange(property, newValue, operation);
                }
                property.currentValue = newValue;
                break;
            case "=":
                newValue = value;
                if (newValue > property.maxValue || newValue < property.minValue) {
                    newValue = HandleOutOfRange(property, newValue, operation);
                }
                property.currentValue = newValue;
                break;
            default:
                Debug.LogError($"不支持的操作: {operation}");
                break;
        }
    }

    private void ExecuteListOperation(string effectCode)
    {
        string[] parts = effectCode.Split(':');
        if (parts.Length < 3 && parts[1].Trim().ToLower() != "clear")
        {
            Debug.LogError($"无效的List操作格式: {effectCode}");
            return;
        }
        
        try {
            string propertyName = parts[0].Trim();
            string operation = parts[1].Trim();
            string elementID = parts[2].Trim();
            if (listGameProperties.TryGetValue(propertyName, out FieldInfo fieldInfo)) {
                ApplyListOperation(fieldInfo, elementID, operation);
                Debug.Log($"执行效果 {effectCode} 成功");
            }
            else {
                Debug.LogError($"未找到属性: {propertyName}");
                return;
            }
        }
        catch (Exception e) {
            Debug.LogError($"执行List操作时出错: {e.Message}");
        }
    }
    
    private void ApplyListOperation(FieldInfo property, string elementID, string operation)
    {
        object listObject = property.GetValue(GameManager.Instance);
        IList list = listObject as IList;
        
        if (list == null)
        {
            Debug.LogError($"属性 {property.Name} 不是一个有效的List");
            return;
        }
        
        object value = elementID;
        switch (operation)
        {
            case "add":
                list.Add(value);
                break;
            case "remove":
                list.Remove(value);
                break;
            case "clear":
                list.Clear();
                break;
            default:
                Debug.LogError($"不支持的List操作: {operation}");
                break;
        }
    }
}    