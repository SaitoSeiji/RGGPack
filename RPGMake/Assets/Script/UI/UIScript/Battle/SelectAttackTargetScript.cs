using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectAttackTargetScript : AbstractUIScript_button
{
    string _mySkillName;

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
        var data = UIController.Instance.GetFlashData("command");
        if (data == null) _mySkillName = "";
        else
        {
            _mySkillName = data._memberSet_st["skillName"];
        }
    }

    protected override List<ButtonData> CreateMyButtonData()
    {
        var dataList = BattleController_mono.Instance.GetEnemyList();
        var resultList = new List<ButtonData>();
        foreach(var data in dataList)
        {
            if (!data.IsAlive()) continue;
            resultList.Add(new ButtonData(data._myCharData._name,CreateClickEvent(data)));
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(EnemyChar data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>BattleUIController.Instance.EndCommand(_mySkillName, data._myCharData._name,_MyUIBase));
        return ue;
    }
}
