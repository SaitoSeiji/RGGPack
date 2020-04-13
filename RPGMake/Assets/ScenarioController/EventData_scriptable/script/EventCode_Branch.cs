using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//使用しているflachData:select
[CreateAssetMenu(menuName = "EventData/Create BranchTextData", fileName = "BranchEventData")]
public class EventCode_Branch : EventCodeScriptable
{
    [SerializeField, Space(20)] List<EventCodeScriptable> _branchCodeList;


    public override EventCodeScriptable GetNextCode()
    {
        if (base.GetNextCode() != null)
        {
            Debug.Log("EventCode_Branch:next code is node used");
        }
        if (_branchCodeList.Count == 0) return null;
        int select = int.Parse(_flashData["select"]);
        _flashData.Remove("select");
        return _branchCodeList[select];
    }
}