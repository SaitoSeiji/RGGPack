using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_useItem : AbstractUIScript
{
    DBData selectData;

    protected override List<ButtonData> CreateMyButtonData()
    {
        return null;
    }

    protected override void ChengedUIStateAction(UIBase.UIState chenged)
    {
        base.ChengedUIStateAction(chenged);
        switch (chenged)
        {
            case UIBase.UIState.ACTIVE:
                selectData = UIController.Instance.GetFlashData("select_item");
                break;
        }
    }

    public void Onclick_useItem()
    {
        var operatedata= ItemDBData.AddHaveNum(selectData._serchId,-1);
        SaveDataController.Instance.SetData<ItemDB>(operatedata);
    }
}
