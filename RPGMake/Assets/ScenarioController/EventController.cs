using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : SingletonMonoBehaviour<EventController>
{
    [SerializeField] EventDataMonoBehaviour firstEvent;

    private void Start()
    {
        if (firstEvent != null) firstEvent.EventAction();
    }

    public bool CoalEvent(EventDataMonoBehaviour data)
    {
        if (data.CheckCoalEnable())
        {
            data.EventAction();
            return true;
        }
        else
        {
            Debug.Log("cant coal Event");
            return false;
        }
    }


    public bool GetReadNow()
    {
        return EventCodeReadController.getIsReadNow;
    }
}
