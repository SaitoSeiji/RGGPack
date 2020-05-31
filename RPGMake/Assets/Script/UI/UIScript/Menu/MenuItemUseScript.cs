using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuItemUseScript : AbstractUIScript_button
{
    ItemDBData _myItemDBData;
    [SerializeField] ChracterFieldDisplayer _menuCharctes;
    [SerializeField] UnityEvent _successEvent;
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

    protected override List<ButtonData> CreateMyButtonData()
    {
        var targetList = _menuCharctes.GetTargetPool(GetMyItemData()._data);

        bool allSelect = !Battle_targetDicide.IsInputSelect(GetMyItemData()._data._targetType);
        var btType = (allSelect) ? ButtonData.ButtonType.Selected : ButtonData.ButtonType.Selectable;
        var result= new List<ButtonData>();
        foreach(var target in targetList)
        {
            //クリックイベントの作成
            UnityEvent ue = new UnityEvent();
            ue.AddListener(() => {
                _menuCharctes.UseItem_menu(GetMyItemData()._data,target);
                _menuCharctes.SyncParam_pl();
                _successEvent?.Invoke();
            });
            //使用可能かどうかの判断
            bool isuseable = Battle_targetResource.IsUseAble(GetMyItemData()._data._targetResource, true, target);
            if (!isuseable) btType = ButtonData.ButtonType.Unselectable;
            //作成
            var add = new ButtonData(target._displayName,ue,btType);
            result.Add(add);
        }
        return result;
    }
}
