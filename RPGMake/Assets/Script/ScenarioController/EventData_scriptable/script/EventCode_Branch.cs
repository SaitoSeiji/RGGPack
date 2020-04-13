using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//使用しているflachData:select
[CreateAssetMenu(menuName = "EventData/Create BranchTextData", fileName = "BranchEventData")]
public class EventCode_Branch : EventCodeScriptable
{
    [SerializeField, Space(20)] List<EventCodeData> _branchCodeList;


    public override EventCodeScriptable GetNextCode()
    {
        if (base.GetNextCode() != null)
        {
            Debug.Log("EventCode_Branch:next code is node used");
        }
        if (_branchCodeList.Count == 0) return null;
        int select = int.Parse(_codeData._flashData["select"]);
        _codeData._flashData.Remove("select");
        var next= _branchCodeList[select]._nextEventCode;

        return CreateNextCode(_branchCodeList[select]._text, next);
    }

    //選択されたEventCodeDataのテキストも追加
    EventCodeScriptable CreateNextCode(string addst,EventCodeScriptable next)
    {
        if (addst == null) return next;
        if (next != null)
        {
            next.AddData(addst);
            return next;
        }
        else
        {
            next = CreateInstance<EventCodeScriptable>();
            next.AddData(addst);
            return next;
        }
    }
}