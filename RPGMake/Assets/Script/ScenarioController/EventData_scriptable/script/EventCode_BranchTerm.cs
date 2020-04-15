using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "EventData/Create BranchTerm", fileName = "BranchTerm")]
public class EventCode_BranchTerm : EventCodeScriptable
{
    [SerializeField,HideInInspector] List<string> _nextEventNameList;
    [SerializeField] List<EventCodeScriptable> _nextEventList;

    public override bool CoalEnable()
    {
        if(base.CoalEnable())
        {
            foreach(var data in _nextEventList)
            {
                if (data.CoalEnable()) return true;
                  
            }
        }
        return false;
    }

    public override EventCodeScriptable GetNextCode()
    {
        for(int i = 0; i < _nextEventList.Count; i++)
        {
            if (_nextEventList[i].CoalEnable())
            {
                return _nextEventList[i];
            }
        }

        return null;
    }

    protected override void UpdateData_next(string id, EventDataOperater.ConvertedText data)
    {
        _nextEventNameList = new List<string>();
        foreach(var d in data._content)
        {
            _nextEventNameList.Add(d);
        }
    }

    public override void UpdateNextEvent(List<EventCodeScriptable> database)
    {
        _nextEventList = new List<EventCodeScriptable>();
        foreach(var n in _nextEventNameList)
        {
            var next= database.Where(x => x.name == n).FirstOrDefault();
            _nextEventList.Add(next);
        }
    }
}
