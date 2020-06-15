using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

public class ShopDBData : StaticDBData
{
    [SerializeField]public List<ItemDBData> _itemList = new List<ItemDBData>();
    [SerializeField, NonEditable] List<string> _itemNameList = new List<string>();
    protected override void UpdateMember_child(TempDBData data)
    {
        _itemNameList = data.GetData_list("item"); 
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        _itemList = new List<ItemDBData>();
        var db = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList;
        foreach (var itemKey in _itemNameList)
        {
            try
            {
                var target = db.Where(x => x._serchId == itemKey).First();
                _itemList.Add(target);
                EditorUtility.SetDirty(this);
            }
            catch(InvalidOperationException e)
            {
                ThrowErrorLog(e, itemKey, ErrorCode_uncollectName);
            }

        }
    }
}
