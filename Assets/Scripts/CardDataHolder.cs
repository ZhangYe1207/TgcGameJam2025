using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDataHolder : MonoBehaviour
{
    public Card cardData;
    public CardDetailsManager CardDetailsManager;
    private ScrollRect parentScrollRect;

    void Start() {
        // 查找父级ScrollRect
        parentScrollRect = GetComponentInParent<ScrollRect>();
        
        // 现有代码保持不变
        EventTrigger et = gameObject.AddComponent<EventTrigger>();
        
        // 添加PointerEnter事件
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { 
            CardDetailsManager.Show(cardData, gameObject.transform.position);
            SoundManager.Instance.Play("HoverOver");
        });
        et.triggers.Add(entry);
        
        // 添加PointerExit事件
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { 
            CardDetailsManager.Hide(); 
        });
        et.triggers.Add(entry);
        
        // 添加拖动事件处理，用于转发给ScrollView
        AddDragEventHandling(et);
    }
    
    private void AddDragEventHandling(EventTrigger et) {
        // 拖动开始事件
        EventTrigger.Entry dragBeginEntry = new EventTrigger.Entry();
        dragBeginEntry.eventID = EventTriggerType.BeginDrag;
        dragBeginEntry.callback.AddListener((data) => {
            // 转发拖动开始事件给ScrollView
            if (parentScrollRect != null) {
                ExecuteEvents.Execute(parentScrollRect.gameObject, (PointerEventData)data, ExecuteEvents.beginDragHandler);
            }
        });
        et.triggers.Add(dragBeginEntry);
        
        // 拖动事件
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => {
            // 转发拖动事件给ScrollView
            if (parentScrollRect != null) {
                ExecuteEvents.Execute(parentScrollRect.gameObject, (PointerEventData)data, ExecuteEvents.dragHandler);
            }
        });
        et.triggers.Add(dragEntry);
        
        // 拖动结束事件
        EventTrigger.Entry dragEndEntry = new EventTrigger.Entry();
        dragEndEntry.eventID = EventTriggerType.EndDrag;
        dragEndEntry.callback.AddListener((data) => {
            // 转发拖动结束事件给ScrollView
            if (parentScrollRect != null) {
                ExecuteEvents.Execute(parentScrollRect.gameObject, (PointerEventData)data, ExecuteEvents.endDragHandler);
            }
        });
        et.triggers.Add(dragEndEntry);
        
        // 滚动事件
        EventTrigger.Entry scrollEntry = new EventTrigger.Entry();
        scrollEntry.eventID = EventTriggerType.Scroll;
        scrollEntry.callback.AddListener((data) => {
            // 转发滚动事件给ScrollView
            if (parentScrollRect != null) {
                ExecuteEvents.Execute(parentScrollRect.gameObject, (PointerEventData)data, ExecuteEvents.scrollHandler);
            }
        });
        et.triggers.Add(scrollEntry);
    }
}