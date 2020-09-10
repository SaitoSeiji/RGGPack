using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

//ショップで販売するものに着ける
//ここで要求しているデータを各自実装することになるので微妙？
public interface IShopContent
{
    (string id,string name,int price,Sprite image) GetShopContentData();
}


public class ShopDBData : StaticDBData
{
    [SerializeField]public List<ItemDBData> _itemList = new List<ItemDBData>();
    [SerializeField]public List<EquipDBData> _equipList = new List<EquipDBData>();
    [SerializeField, NonEditable] List<string> _itemNameList = new List<string>();
    [SerializeField, NonEditable] List<string> _equipNameList = new List<string>();
    protected override void UpdateMember_child(TempDBData data)
    {
        _itemNameList = data.GetData_list("item");
        _equipNameList = data.GetData_list("equip");
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        var itemdb = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList;
        _itemList = SetList(_itemNameList, itemdb);
        var equipdb = SaveDataController.Instance.GetDB_static<EquipDB>()._dataList;
        _equipList = SetList(_equipNameList, equipdb);
        //foreach (var itemKey in _itemNameList)
        //{
        //    try
        //    {
        //        var target = db.Where(x => x._serchId == itemKey).First();
        //        _itemList.Add(target);
        //        EditorUtility.SetDirty(this);
        //    }
        //    catch(InvalidOperationException e)
        //    {
        //        ThrowErrorLog(e, itemKey, ErrorCode_uncollectName);
        //    }

        //}
    }

    List<T> SetList<T>(List<string> keyList,List<T> db)
        where T:StaticDBData
    {
        var result = new List<T>();
        foreach (var key in keyList)
        {
            try
            {
                var target = db.Where(x => x._serchId == key).First();
                result.Add(target);
                EditorUtility.SetDirty(this);
            }
            catch (InvalidOperationException e)
            {
                ThrowErrorLog(e, key, ErrorCode_uncollectName);
            }
        }
        return result;
    }
}
