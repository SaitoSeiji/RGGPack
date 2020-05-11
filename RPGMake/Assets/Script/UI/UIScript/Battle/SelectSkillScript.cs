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
        var useAbleSkillList = BattleController_mono.Instance.GetSkillList();
        var resultList = new List<ButtonData>();
        foreach (var data in useAbleSkillList)
        {
            resultList.Add(new ButtonData(data._Data._skillName, CreateClickEvent(data)));
        }
        return resultList;
    }

    private UnityEvent CreateClickEvent(SkillDBData data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>ClickNextUIEvent(data));
        return ue;
    }

    void ClickNextUIEvent(SkillDBData data)
    {
        var ct = BattleController_mono.Instance.battle.GetCommantTarget(data._Data._skillName);
        //対象選択をする場合
        if (ct.IsInputSelect())
        {
            UIController.Instance.SetFlashData("command", data);
            _MyUIBase.AddUI(_targetSelectUI);
        }
        else//しない場合
        {
            BattleUIController.Instance.EndCommand(data._Data._skillName, null, _MyUIBase);
        }
    }
}
