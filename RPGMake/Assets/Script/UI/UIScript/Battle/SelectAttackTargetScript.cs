using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectAttackTargetScript : AbstractUIScript_button
{

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
    }

    string GetSkillName()
    {
        //使用するスキル名を取得
        var data = UIController.Instance.GetFlashData_static("command") as SkillDBData;
        if (data == null) return "";
        else
        {
            return data._Data._skillName;
        }
    }

    protected override List<ButtonData> CreateMyButtonData()
    {
        var skillName = GetSkillName();
        var dataList = BattleController_mono.Instance.battle.GetCommantTarget(skillName).GetTargetPool();
        var resultList = new List<ButtonData>();
        foreach(var data in dataList)
        {
            if (!data.IsAlive()) continue;
            resultList.Add(new ButtonData(data._myCharData._name,CreateClickEvent(data,skillName)));
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(BattleChar target,string skillName)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>BattleUIController.Instance.EndCommand(skillName, target._myCharData._name,_MyUIBase));
        return ue;
    }
}
