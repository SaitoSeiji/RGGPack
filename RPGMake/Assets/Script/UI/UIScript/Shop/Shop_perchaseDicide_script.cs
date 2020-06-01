using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shop_perchaseDicide_script : AbstractUIScript_onclick
{
    ItemDBData _myitemData;
    int purchaseNum;

    [SerializeField] UnityEvent successAction;

    void Init()
    {
        if (_myitemData == null)
        {
            _myitemData = (ItemDBData)UIController.Instance.GetFlashData_static("item");
            purchaseNum =int.Parse( UIController.Instance.GetFlashData_string("num"));
        }
    }

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
        Init();
    }
    protected override void ChengeState_toClose()
    {
        base.ChengeState_toClose();
        _myitemData = null;
    }
    public override void OnclickAction()
    {
        Init();
        ShopDB.BuyItem(_myitemData,purchaseNum);
        successAction?.Invoke();
    }
}
