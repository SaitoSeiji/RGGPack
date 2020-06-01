using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop_perchaseNum_script : AbstractUIScript_button
{
    [SerializeField] TextMeshProUGUI _priceText;
    [SerializeField] UIBase _nextUI;
    protected override List<ButtonData> CreateMyButtonData()
    {
        var itemData = (ItemDBData)UIController.Instance.GetFlashData_static("item");
        var result= new List<ButtonData>();
        int maxNum = ShopDB.GetBuyableCount(itemData);
        for(int i = 1; i <= maxNum; i++)
        {
            UnityEvent click = new UnityEvent();
            int num = i;
            click.AddListener(() =>
            {
                UIController.Instance.SetFlashData("num", num.ToString());
                UIController.Instance.SetFlashData("item", itemData);
                _MyUIBase.AddUI(_nextUI);
            });
            var price = itemData._data._price * i;
            Action cursor = () =>
            {
                _priceText.text = $"{price}G";
            };
            var add = new ButtonData(i.ToString(),click,cursor,ButtonData.ButtonType.Selectable);
            result.Add(add);
        }
        return result;
    }
}
