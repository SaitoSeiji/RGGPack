using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Shop_perchace_script : AbstractUIScript_button
{
    [SerializeField] UIBase _nextUI;

    [SerializeField] GameObject panel;
    [SerializeField] Text haveMoney;

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
        panel.SetActive(true);
        var db = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
        haveMoney.text = $"所持金:{db._haveMoney}G";
    }

    protected override void ChengeState_toClose()
    {
        base.ChengeState_toClose();
        panel.SetActive(false);
    }

    protected override List<ButtonData> CreateMyButtonData()
    {
        var result= new List<ButtonData>(); ;
        foreach (var item in ResourceDB_mono.Instance._nowShopData._itemList)
        {
            var bttype = (ShopDB.GetBuyableCount(item) == 0) ? ButtonData.ButtonType.Unselectable : ButtonData.ButtonType.Selectable;
            var clickAction = new UnityEvent();
            clickAction.AddListener(()=> {
                UIController.Instance.SetFlashData("item",item);
                _MyUIBase.AddUI(_nextUI);
            });
            var add = new ButtonData(item._data._displayName,clickAction,bttype);
            add._buttonImage = item._data._itemImage;
            add._additonalText = $"{item._data._price}G";
            result.Add(add);
        }
        return result;
    }
    
}
