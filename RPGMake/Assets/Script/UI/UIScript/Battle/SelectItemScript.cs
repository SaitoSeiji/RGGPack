using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;
using TMPro;

public class SelectItemScript : AbstractUIScript_button
{
    [SerializeField] UIBase _targetSelectUI;

    [SerializeField, Space(10)] GameObject _textPannel;
    [SerializeField] TextMeshProUGUI setumeiText;

    protected override void ChengeState_toClose()
    {
        base.ChengeState_toClose();
        _textPannel.SetActive(false);
        setumeiText.text = "";
    }

    protected override List<ButtonData> CreateMyButtonData()
    {
        var partyDB = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0]._haveItemList
            .Where(x=>x.haveNum>0).ToList();
        var itemDB = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList;
        var resultList = new List<ButtonData>();

        foreach(var partyItemData in partyDB)
        {
            //partyDBにあるアイテムデータは個数はあるがimage等を保存できていないので、生のアイテムデータを取りに行っている
            var itemDBData = itemDB.Where(x => x._serchId == partyItemData._itemData._serchId).First();
            var data = new ButtonData(itemDBData._data._displayName,CreateClickEvent(itemDBData),
                                      CreatCursorEvent(itemDBData),
                                      ButtonData.ButtonType.Selectable);
            data._additonalText =$"×{partyItemData.haveNum}" ;
           
            data._buttonImage = itemDBData._data._itemImage;
            resultList.Add(data);
        }
        return resultList;
    }

    UnityEvent CreateClickEvent(ItemDBData itemData)
    {

        //var ct = BattleController_mono.Instance.battle.GetTargetPool(itemData._data);
        //UnityEvent ue = new UnityEvent();
        ////if (ct.IsInputSelect())
        //if (Battle_targetDicide.IsInputSelect(itemData._data._targetType))
        //    {
        //    ue.AddListener(() => {
        //        UIController.Instance.SetFlashData("item", itemData);
        //        _MyUIBase.AddUI(_targetSelectUI);
        //    });
        //}
        //else
        //{
        //    ue.AddListener(()=> {
        //        BattleUIController.Instance.EndCommand(itemData._data, null, _MyUIBase);
        //    });
        //}
        //return ue;


        UnityEvent ue = new UnityEvent()
            ; ue.AddListener(() => {
                UIController.Instance.SetFlashData("item", itemData);
                _MyUIBase.AddUI(_targetSelectUI);
            });
        return ue;

    }

    Action CreatCursorEvent(ItemDBData itemData)
    {
        Action act = () =>
        {
            _textPannel.SetActive(true);
            var title = $"{itemData._data._displayName}\n" +
            $"{itemData._data._explanation}";
            setumeiText.text = title;
        };

        return act;
    }
}
