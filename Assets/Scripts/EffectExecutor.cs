using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EffectExecutor : MonoBehaviour
{
    private Dictionary<string, PropertyInfo> gameProperties = new Dictionary<string, PropertyInfo>();
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
        
        InitializeGameProperties();
    }

    private void InitializeGameProperties()
    {
        Type gameStateType = typeof(GameState);
        PropertyInfo[] properties = gameStateType.GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            
        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite && IsSupportedType(property.PropertyType))
            {
                gameProperties[property.Name] = property;
            }
        }

        Debug.Log("初始化游戏属性: " + string.Join(", ", gameProperties.Keys));
    }

    private bool IsSupportedType(Type type)
    {
        // 基础类型支持
        if (type == typeof(int))
            return true;
            
        // List类型支持
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return true;
            
        return false;
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
            string[] parts = effectCode.Split(':');
            if (parts.Length < 3)
            {
                Debug.LogError($"无效的List操作格式: {effectCode}");
                return;
            }
            
            string propertyName = parts[0].Trim().ToLower();
            string operation = parts[1].Trim().ToLower();
            string valueString = parts[2].Trim();
            
            if (gameProperties.TryGetValue(propertyName, out PropertyInfo property))
            {
                try
                {
                    Type elementType = property.PropertyType.GetGenericArguments()[0];
                    object value = Convert.ChangeType(valueString, elementType);
                    ApplyListOperation(property, value, operation);
                }
                catch (Exception e)
                {
                    Debug.LogError($"执行List操作时出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"未找到属性: {propertyName}");
            }
            return;
        }
        
        // 基础类型操作解析
        string[] parts = effectCode.Split(new[] { '+', '-', '*', '/', '=' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            Debug.LogError($"无效的效果代码: {effectCode}");
            return;
        }

        string propertyNameBase = parts[0].Trim().ToLower();
        string valueString = parts[1].Trim();

        if (gameProperties.TryGetValue(propertyNameBase, out PropertyInfo property))
        {
            try
            {
                object value = ConvertToType(valueString, property.PropertyType);
                ApplyOperation(property, value, effectCode);
            }
            catch (Exception e)
            {
                Debug.LogError($"执行效果时出错: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"未找到属性: {propertyNameBase}");
        }
    }

    private void ApplyListOperation(PropertyInfo property, object value, string operation)
    {
        object listObject = property.GetValue(GameState.Instance);
        IList list = listObject as IList;
        
        if (list == null)
        {
            Debug.LogError($"属性 {property.Name} 不是一个有效的List");
            return;
        }
        
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

    private object ConvertToType(string valueString, Type targetType)
    {
        if (targetType == typeof(int))
            return int.Parse(valueString);
        if (targetType == typeof(float))
            return float.Parse(valueString);
        if (targetType == typeof(bool))
            return bool.Parse(valueString);
        if (targetType == typeof(string))
            return valueString;
        return null;
    }

    private void ApplyOperation(PropertyInfo property, object value, string effectCode)
    {
        object currentValue = property.GetValue(GameState.Instance);
        
        if (effectCode.Contains("+="))
        {
            if (property.PropertyType == typeof(int))
                property.SetValue(GameState.Instance, (int)currentValue + (int)value);
            else if (property.PropertyType == typeof(float))
                property.SetValue(GameState.Instance, (float)currentValue + (float)value);
        }
        else if (effectCode.Contains("-="))
        {
            if (property.PropertyType == typeof(int))
                property.SetValue(GameState.Instance, (int)currentValue - (int)value);
            else if (property.PropertyType == typeof(float))
                property.SetValue(GameState.Instance, (float)currentValue - (float)value);
        }
        else if (effectCode.Contains("*="))
        {
            if (property.PropertyType == typeof(int))
                property.SetValue(GameState.Instance, (int)currentValue * (int)value);
            else if (property.PropertyType == typeof(float))
                property.SetValue(GameState.Instance, (float)currentValue * (float)value);
        }
        else if (effectCode.Contains("/="))
        {
            if (property.PropertyType == typeof(int))
                property.SetValue(GameState.Instance, (int)currentValue / (int)value);
            else if (property.PropertyType == typeof(float))
                property.SetValue(GameState.Instance, (float)currentValue / (float)value);
        }
        else if (effectCode.Contains("="))
        {
            property.SetValue(GameState.Instance, value);
        }
    }
}    