using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

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
        var result= new List<ButtonData>();
        var targetShopData = ResourceDB_mono.Instance._nowShopData;
        List<IShopContent> _contentList = new List<IShopContent>();
        _contentList.AddRange(targetShopData._itemList.Select(x => (IShopContent)x));
        _contentList.AddRange(targetShopData._equipList.Select(x => (IShopContent)x));
        //_contentListの作成
        foreach (var content in _contentList)
        {
            var bttype = (ShopDB.GetBuyableCount(content) == 0) ? ButtonData.ButtonType.Unselectable : ButtonData.ButtonType.Selectable;
            var targetData = content.GetShopContentData();
            var clickAction = new UnityEvent();
            clickAction.AddListener(() => {
                var item = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList.Where(x => x._serchId == targetData.id).FirstOrDefault();//冗長なので後で修正 itemDataを利用しないように七ア
                UIController.Instance.SetFlashData("item", item);//アイテムしか登録できてない
                _MyUIBase.AddUI(_nextUI);
            });
            var add = new ButtonData(targetData.name, clickAction, bttype);
            add._buttonImage = targetData.image;
            add._additonalText = $"{targetData.price}G";
            result.Add(add);
        }
        return result;

        //foreach (var item in ResourceDB_mono.Instance._nowShopData._itemList)
        //{
        //    var bttype = (ShopDB.GetBuyableCount(item) == 0) ? ButtonData.ButtonType.Unselectable : ButtonData.ButtonType.Selectable;
        //    var clickAction = new UnityEvent();
        //    clickAction.AddListener(()=> {
        //        UIController.Instance.SetFlashData("item",item);
        //        _MyUIBase.AddUI(_nextUI);
        //    });
        //    var add = new ButtonData(item._data._displayName,clickAction,bttype);
        //    add._buttonImage = item._data._itemImage;
        //    add._additonalText = $"{item._data._price}G";
        //    result.Add(add);
        //}
        //return result;
    }
    
}
