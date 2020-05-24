using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectAttackTargetScript : AbstractUIScript_button
{
    
    SkillCommandData GetSkillData()
    {
        //使用するスキル名を取得
        var data = UIController.Instance.GetFlashData_static("command") as SkillDBData;
        if (data == null) return null;
        else
        {
            return data._Data;
        }
    }

    protected override List<ButtonData> CreateMyButtonData()
    {
        var commandData = GetSkillData();
        //var dataList = BattleController_mono.Instance.battle.GetCommandTargetDicide(commandData).GetTargetPool();
        var dataList = BattleController_mono.Instance.battle.GetTargetPool(commandData);
        var resultList = new List<ButtonData>();
        foreach(var data in dataList)
        {
            if (!data.IsAlive()) continue;
            resultList.Add(new ButtonData(data._myCharData._name,CreateClickEvent(data,commandData)));
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(BattleChar target,SkillCommandData data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>BattleUIController.Instance.EndCommand(data, target._myCharData._name,_MyUIBase));
        return ue;
    }
}
