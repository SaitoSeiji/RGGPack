using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuItemUseScript : AbstractUIScript_onclick
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

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
        GetMyItemData();
    }

    private void OnDisable()
    {
        _myItemDBData = null;
    }

    //ここでやっていることを汎用化する
    public override void OnclickAction()
    {
        var strtegy= CommandStrategy.GetStrategy(GetMyItemData()._data);
        var user=new PlayerChar( SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>()[0]);
        var friends = new List<BattleChar>() { user };
        var targetList = Battle_targetDicide.GetTargetPool(GetMyItemData()._data._targetType, user,friends,null);
        var target = targetList[0];
        strtegy.TurnAction(user,target,GetMyItemData()._data,friends:friends);
        SaveDataController.Instance.SetData<PlayerDB, SavedDBData_player>(user._myCharData);
    }
}
