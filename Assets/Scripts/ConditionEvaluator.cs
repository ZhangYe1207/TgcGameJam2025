using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConditionEvaluator : MonoBehaviour
{
    private Dictionary<string, PropertyInfo> gameProperties = new Dictionary<string, PropertyInfo>();
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
        
        InitializeGameProperties();
    }

    private void InitializeGameProperties()
    {
        Type gameStateType = typeof(GameState);
        PropertyInfo[] properties = gameStateType.GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            
        foreach (PropertyInfo property in properties)
        {
            if (property.CanRead && IsSupportedType(property.PropertyType))
            {
                gameProperties[property.Name.ToLower()] = property;
            }
        }
    }

    private bool IsSupportedType(Type type)
    {
        // 仅支持int和List<string>类型
        if (type == typeof(int))
            return true;
            
        // List类型支持
        if (type.IsGenericType && 
            type.GetGenericTypeDefinition() == typeof(List<>) &&
            type.GetGenericArguments()[0] == typeof(string))
            return true;
            
        return false;
    }

    public static bool EvaluateCondition(string condition)
    {
        return instance.InternalEvaluateCondition(condition);
    }

    private bool InternalEvaluateCondition(string condition)
    {
        // 处理List包含检查（新格式：listName:has:xxx）
        if (condition.Contains(":has:"))
            return EvaluateListContains(condition);

        // 处理数值比较
        if (condition.Contains(">="))
            return EvaluateComparison(condition, ">=");
        if (condition.Contains("<="))
            return EvaluateComparison(condition, "<=");
        if (condition.Contains(">"))
            return EvaluateComparison(condition, ">");
        if (condition.Contains("<"))
            return EvaluateComparison(condition, "<");
        if (condition.Contains("=="))
            return EvaluateComparison(condition, "==");
        if (condition.Contains("!="))
            return EvaluateComparison(condition, "!=");

        Debug.LogError($"不支持的条件格式: {condition}");
        return false;
    }

    private bool EvaluateListContains(string condition)
    {
        string[] parts = condition.Split(new[] { ":has:" }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            Debug.LogError($"List包含条件解析错误: {condition}");
            return false;
        }

        string listName = parts[0].Trim().ToLower();
        string elementValue = parts[1].Trim();

        if (gameProperties.TryGetValue(listName, out PropertyInfo property))
        {
            try
            {
                // 获取List实例
                IList list = property.GetValue(GameState.Instance) as IList;
                if (list == null)
                {
                    Debug.LogError($"List属性为空: {listName}");
                    return false;
                }

                // 检查List是否包含该元素
                return list.Contains(elementValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"List包含检查时出错: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError($"未找到List属性: {listName}");
            return false;
        }
    }

    private bool EvaluateComparison(string condition, string operatorStr)
    {
        string[] parts = condition.Split(new[] { operatorStr }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            Debug.LogError($"条件解析错误: {condition}");
            return false;
        }

        string propertyName = parts[0].Trim().ToLower();
        string valueString = parts[1].Trim();

        if (gameProperties.TryGetValue(propertyName, out PropertyInfo property))
        {
            try
            {
                // 仅处理int类型
                if (property.PropertyType == typeof(int))
                {
                    int propertyValue = (int)property.GetValue(GameState.Instance);
                    int compareValue = int.Parse(valueString);
                    
                    return CompareIntValues(propertyValue, compareValue, operatorStr);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"条件评估时出错: {e.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogError($"未找到属性: {propertyName}");
            return false;
        }
        
        return false;
    }

    private bool CompareIntValues(int left, int right, string operatorStr)
    {
        switch (operatorStr)
        {
            case ">=": return left >= right;
            case "<=": return left <= right;
            case ">": return left > right;
            case "<": return left < right;
            case "==": return left == right;
            case "!=": return left != right;
        }
        
        return false;
    }
}    