using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_useItem : AbstractUIScript_onclick
{
    TempDBData selectData;


    public override void OnclickAction()
    {
        //var operatedata = ItemDBData.AddHaveNum(selectData._serchId, -1);
        //SaveDataController.Instance.SetData<ItemDB>(operatedata);
    }

    protected override void ChengeState_toActive()
    {
        //base.ChengeState_toActive();
        //selectData = UIController.Instance.GetFlashData("select_item");
    }
}
