using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class SelectSkillScript : AbstractUIScript_button
{
    [SerializeField] UIBase _targetSelectUI;

    protected override List<ButtonData> CreateMyButtonData()
    {
        var saveddb = SaveDataController.Instance.GetDB_static<SkillDB>()._dataList;
        var dataList = BattleController_mono.Instance.GetSkillList();
        var resultList = new List<ButtonData>();
        foreach (var data in dataList)
        {
            var dbData = saveddb.Where(x => x._Data._serchId == data._Data._serchId).First();
            resultList.Add(new ButtonData(data._SKill._skillName, CreateClickEvent(dbData._Data)));
        }
        return resultList;
    }

    private UnityEvent CreateClickEvent(DBData data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>ClickNextUIEvent(data._memberSet_st["skillName"],data));
        return ue;
    }

    void ClickNextUIEvent(string skillName,DBData data)
    {
        var ct = BattleController_mono.Instance.battle.GetCommantTarget(skillName);
        //対象選択をする場合
        if (ct.IsInputSelect())
        {
            UIController.Instance.SetFlashData("command", data);
            _MyUIBase.AddUI(_targetSelectUI);
        }
        else//しない場合
        {
            BattleUIController.Instance.EndCommand(skillName,null, _MyUIBase);
        }
    }
}
