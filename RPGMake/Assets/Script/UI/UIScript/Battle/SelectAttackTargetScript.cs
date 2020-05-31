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
        var targetPool = BattleController_mono.Instance.battle.GetTargetPool(commandData);
        var resultList = new List<ButtonData>();

        bool allSelect = !Battle_targetDicide.IsInputSelect(commandData._target);
        var btType = (allSelect) ? ButtonData.ButtonType.Selected : ButtonData.ButtonType.Selectable;
        foreach (var target in targetPool)
        {
            if (!target.IsAlive()) continue;
            //使用可能かどうかの判断
            bool isuseable = Battle_targetResource.IsUseAble(commandData._TargetResourceType, true, target);
            if (!isuseable) btType = ButtonData.ButtonType.Unselectable;

            resultList.Add(new ButtonData(target._displayName,
                                          CreateClickEvent(target,commandData),
                                          btType));
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(BattleChar target,SkillCommandData data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>BattleUIController.Instance.EndCommand(data, target._displayName,_MyUIBase));
        return ue;
    }
}
