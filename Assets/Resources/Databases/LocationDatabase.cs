using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "LocationDatabase", menuName = "Game/Location Database")]
public class LocationDatabase : ScriptableObject
{
    public List<Location> locations;

    private void OnEnable()
    {
    }

    // 获取当前level的locations
    public List<Location> GetLocationsByLevel(int currentLevel) {
        return locations.Where(l => l.level == currentLevel).ToList();
    }

    // 随机获取Locations
    public List<Location> GetRandomLocationsByLevel(int currentLevel, int count)
    {
        List<Location> locs = GetLocationsByLevel(currentLevel);
        return ListRandomizer.RandomSelectWithoutRepeat<Location>(locs, count);
    }

    public void AddLocation(Location location) {
        locations.Add(location);
    }

    public void ClearLocations() {
        locations.Clear();
    }
}

public static class ListRandomizer
{
    // 从列表中随机选择n个不重复的元素
    public static List<T> RandomSelectWithoutRepeat<T>(List<T> list, int n)
    {
        if (list == null || list.Count == 0 || n <= 0)
            return new List<T>();

        // 如果n大于列表长度，调整为列表长度
        n = Math.Min(n, list.Count);
        
        List<T> result = new List<T>();
        List<T> tempList = new List<T>(list); // 复制原列表
        
        for (int i = 0; i < n; i++)
        {
            // 随机选择一个索引
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex); // 移除已选元素，确保不重复
        }
        
        return result;
    }

    // 从列表中随机选择n个元素（可重复）
    public static List<T> RandomSelectWithRepeat<T>(List<T> list, int n)
    {
        if (list == null || list.Count == 0 || n <= 0)
            return new List<T>();

        List<T> result = new List<T>();
        
        for (int i = 0; i < n; i++)
        {
            // 随机选择一个索引
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            result.Add(list[randomIndex]);
        }
        
        return result;
    }
}