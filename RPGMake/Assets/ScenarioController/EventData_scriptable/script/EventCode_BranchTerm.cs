using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EventData/Create BranchTerm", fileName = "BranchTerm")]
public class EventCode_BranchTerm : EventCodeScriptable
{
    [SerializeField,Space(20)]List<EventCodeScriptablesTerm> _termList;
    [SerializeField] List<EventCodeScriptable> _eventList;

    public override bool CoalEnable()
    {
        if(base.CoalEnable())
        {
            foreach(var data in _eventList)
            {
                if (data.CoalEnable()) return true;
                  
            }
        }
        return false;
    }

    public override EventCodeScriptable GetNextCode()
    {
        for(int i = 0; i < _eventList.Count; i++)
        {
            if (_eventList[i].CoalEnable())
            {
                return _eventList[i];
            }
        }

        return null;
    }
}
