using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectItemTargetScript : AbstractUIScript_button
{
    ItemDBData _myItemDBData;

    ItemDBData GetMyItemData()
    {
        if (_myItemDBData == null)
        {
            var data = UIController.Instance.GetFlashData_static("item");
            _myItemDBData = data as ItemDBData;
        }

        return _myItemDBData;
    }
    private void OnDisable()
    {
        _myItemDBData = null;
    }


    protected override List<ButtonData> CreateMyButtonData()
    {
        //var targetPool = BattleController_mono.Instance.battle.GetTargetPool(GetMyItemData()._data).GetTargetPool();
        var targetPool = BattleController_mono.Instance.battle.GetTargetPool(GetMyItemData()._data);
        var result = new List<ButtonData>();
        bool allSelect = !Battle_targetDicide.IsInputSelect(GetMyItemData()._data._targetType);
        var btType = (allSelect) ? ButtonData.ButtonType.Selected : ButtonData.ButtonType.Selectable;
        foreach (var target in targetPool)
        {
            //使用可能かどうかの判断

            bool isuseable = Battle_targetResource.IsUseAble(GetMyItemData()._data._targetResource, true, target);
            if (!isuseable) btType = ButtonData.ButtonType.Unselectable;

            result.Add(new ButtonData(target._displayName,
                                      CreateClickEvent(GetMyItemData()._data,target),
                                      btType));
        }
        return result;
    }

    UnityEvent CreateClickEvent(ItemData data,BattleChar target)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(() => BattleUIController.Instance.EndCommand(data, target._displayName, _MyUIBase));
        return ue;
    }
}
