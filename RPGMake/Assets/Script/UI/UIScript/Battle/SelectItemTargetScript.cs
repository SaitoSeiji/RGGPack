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
        foreach (var target in targetPool)
        {
            result.Add(new ButtonData(target._myCharData._name,CreateClickEvent(GetMyItemData()._data,target)));
        }
        return result;
    }

    UnityEvent CreateClickEvent(ItemData data,BattleChar target)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(() => BattleUIController.Instance.EndCommand(data, target._myCharData._name, _MyUIBase));
        return ue;
    }
}
