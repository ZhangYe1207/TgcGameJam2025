using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public List<RandomEvent> GetEventsByLevel(int level) {
        return allEvents.Where(e => e.level == level).ToList();
    }

    private void InitEvents() {
        foreach (var e in allEvents) {
            Debug.Log(e.eventId);
        }
    }

}