using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventDatabase", menuName = "Game/Event Database")]
public class EventDatabase : ScriptableObject
{
    public List<RandomEvent> allEvents;

    private void OnEnable()
    {
        InitEvents();
    }

    // 根据ID查找事件
    public RandomEvent GetEventByID(string id) {
        return allEvents.Find(e => e.eventId == id);
    }

    private void InitEvents() {
        foreach (var e in allEvents) {
            Debug.Log(e.eventId);
        }
    }

}