using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Test_uiScript : AbstractUIScript_button
{
    [SerializeField] UIBase _nextUIBase;

    protected override List<ButtonData> CreateMyButtonData()
    {
        var db = SaveDataController.Instance.GetDB_var<ItemDB,SavedDBData_item>();
        var result = new List<ButtonData>();
        foreach (var data in db)
        {
            //if (data._memberSet_int["haveNum"] <= 0) continue;
            //result.Add(new ButtonData(data._memberSet_st["displayName"], CreateClickEvent(data)));
        }
        return result;
    }

    UnityEvent CreateClickEvent(SkillDBData data)
    {
        UnityEvent ev = new UnityEvent();
        ev.AddListener(() => UIController.Instance.SetFlashData("select_item", data));
        ev.AddListener(()=>_MyUIBase.AddUI(_nextUIBase));
        return ev;
    }
}
