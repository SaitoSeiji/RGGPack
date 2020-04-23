using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectAttackTargetScript : AbstractUIScript_button
{
    protected override List<ButtonData> CreateMyButtonData()
    {
        var dataList = BattleController_mono.Instance.GetEnemyList();
        var resultList = new List<ButtonData>();
        foreach(var data in dataList)
        {
            resultList.Add(new ButtonData(data._myCharData._name,CreateClickEvent(data)));
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(EnemyChar data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>BattleUIController.Instance.EndCommand(0,0,_MyUIBase));
        return ue;
    }
}
