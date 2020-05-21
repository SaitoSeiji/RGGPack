using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;



[System.Serializable]
public class SavedDBData_party : SavedDBData
{
    [System.Serializable]
    public class PartyItemData
    {
        public ItemDBData _itemData;
        public int haveNum;

        public PartyItemData(ItemDBData data,int num)
        {
            _itemData = data;
            haveNum = num;
        }
    }

    [SerializeField] public List<PartyItemData> _haveItemList = new List<PartyItemData>();

    public void ChengeItemNum(string key,int num)
    {
        try
        {

            var target = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList.Where(x => x._serchId == key).First();
            if (_haveItemList.Where(x=>x._itemData==target).FirstOrDefault()==null)
            {
                _haveItemList.Add(new PartyItemData(target,0));
            }
            _haveItemList.ForEach(x => {
                if (x._itemData == target)
                {
                    x.haveNum += num;
                    x.haveNum = Mathf.Clamp(x.haveNum, 0, 100);
                }
            });
        }
        catch(Exception e)
        {
            Debug.LogError($"item key is not exist :{key}\n" +
                $"{e}");
        }
    }
    
    public int GetHaveItemNum(string key)
    {
        var target = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList.Where(x => x._serchId == key).First();
        if (_haveItemList.Where(x => x._itemData == target).FirstOrDefault() == null)
        {
            return 0;
        }
        var result = 0;
        _haveItemList.ForEach(x => {
            if (x._itemData == target)
            {
                result = x.haveNum;
            }
        });
        return result;
    }
}

public class PartyDBData : VariableDBData
{
    [SerializeField] SavedDBData_party _partyData=new SavedDBData_party();
    [SerializeField, NonEditable] List<string> _haveItemKeyList = new List<string>();//space区切り

    protected override SavedDBData GetSavedDBData_child()
    {
        return _partyData;
    }

    protected override void UpdateMember_child(TempDBData data)
    {
        _haveItemKeyList = data.GetData_list("haveItem");
    }


    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        _partyData._haveItemList = new List<SavedDBData_party.PartyItemData>();
        var db = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList;
        foreach(var st in _haveItemKeyList)
        {
            var input = SplitItemKey(st);
            var itemName = input[0];
            var itemNum =int.Parse(input[1]);
            try
            {
                _partyData.ChengeItemNum(itemName,itemNum);
            }
            catch (Exception e)
            {
                Debug.LogError($"item key is not exist\n" +
                    $"{e}");
            }
        }
    }

    static string[] SplitItemKey(string data)
    {
        //入力データ itemkey itemnum
        var input = data.Split(' ');
        if (input.Length != 2)
        {
            Debug.LogError($"have item data is uncorrect");
        }

        try
        {
            int.Parse(input[1]);
        }
        catch
        {
            Debug.LogError($"have item data is uncorrect");
        }

        return input;
    }
}
