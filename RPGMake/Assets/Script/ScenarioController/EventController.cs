using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class EventController : SingletonMonoBehaviour<EventController>
{
    [SerializeField] EventDB _eventDataBase;

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
        return EventCodeReadController._getIsReadNow;
    }

    public EventCodeScriptable GetEventData(string id)
    {
        return _eventDataBase._scriptableList.Where(x => x.name == id).FirstOrDefault();
    }

    //editorのみ
#if UNITY_EDITOR
    public void SetEventDataBase(EventDB db)
    {
        _eventDataBase = db;
    }
#endif
}
