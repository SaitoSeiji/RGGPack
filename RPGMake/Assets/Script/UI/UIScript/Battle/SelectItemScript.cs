using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class SelectItemScript : AbstractUIScript_button
{
    [SerializeField] UIBase _targetSelectUI;

    protected override List<ButtonData> CreateMyButtonData()
    {
        var db = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0]._haveItemList
            .Where(x=>x.haveNum>0).ToList();
        var resultList = new List<ButtonData>();

        foreach(var item in db)
        {
            resultList.Add(new ButtonData(
                item._itemData._data._displayName,
                CreateClickEvent(item._itemData)));
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(ItemDBData itemData)
    {

        var ct = BattleController_mono.Instance.battle.GetCommandTargetDicide(itemData._data);
        UnityEvent ue = new UnityEvent();
        if (ct.IsInputSelect())
        {
            ue.AddListener(() => {
                UIController.Instance.SetFlashData("item", itemData);
                _MyUIBase.AddUI(_targetSelectUI);
            });
        }
        else
        {
            ue.AddListener(()=> {
                BattleUIController.Instance.EndCommand(itemData._data, null, _MyUIBase);
            });
        }
        return ue;
    }
}
