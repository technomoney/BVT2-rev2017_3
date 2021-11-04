using UnityEngine;
using UnityEngine.EventSystems;

public class Testing_EventData : MonoBehaviour
{
    private EventTrigger.Entry entry, entry2, entry3;

    public EventTrigger eventTrigger;

    // Use this for initialization
    private void Start()
    {
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerDown;
        entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(Clicked);
        entry2.callback.AddListener(CDown);
        entry3.callback.AddListener(Up);
        eventTrigger.triggers.Add(entry);
        eventTrigger.triggers.Add(entry2);
        eventTrigger.triggers.Add(entry3);
    }

    private void Clicked(BaseEventData d)
    {
        Debug.Log("Clicked");
    }

    private void CDown(BaseEventData d)
    {
        Debug.Log("Down");
    }

    private void Up(BaseEventData d)
    {
        Debug.Log("Up");
    }
}