using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class MasterButton : MonoBehaviour
{
    [Serializable] public class ButtonPressEvent : UnityEvent { }
    [Serializable] public class ButtonReleaseEvent : UnityEvent { }

    public ButtonPressEvent onButtonPressed;
    public ButtonReleaseEvent onButtonReleased;

    public string defaultText = "Press to Talk";
    public string pressedText = "Release to Drop";

    private EventTrigger eventTrigger;
    private TMP_Text buttonText;

    void Awake()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        buttonText.text = defaultText;

        eventTrigger = GetComponent<EventTrigger>();
        AddEventTrigger(EventTriggerType.PointerDown, () => { onButtonPressed.Invoke(); buttonText.text = pressedText; });
        AddEventTrigger(EventTriggerType.PointerUp, () => { onButtonReleased.Invoke(); buttonText.text = defaultText; });
    }

    void AddEventTrigger(EventTriggerType type, Action callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((eventData) => { callback(); });

        eventTrigger.triggers.Add(entry);
    }
}
