using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;



[Serializable]
public class SavedDBData_party : SavedDBData
{
    [Serializable]
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
    [Serializable]
    public class PostionData
    {
        [SerializeField] public Vector2 _pos;//位置
        [SerializeField] public Player.DIRECTION _direction;
        [SerializeField] public string _mapName;
        public PostionData(Vector2 pos,Player.DIRECTION dir,string mapName)
        {
            _pos = pos;
            _direction = dir;
            _mapName = mapName;
        }
    }
    [SerializeField] public int _haveMoney;
    [SerializeField] public List<PartyItemData> _haveItemList = new List<PartyItemData>();
    [SerializeField] PostionData _postionData;
    public PostionData _PostionData
    {
        get { return _postionData; }
        set { _postionData = value; }
    }
    #region item
    public void ChengeItemNum(string key, int num)
    {
        try
        {
            var target = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList.Where(x => x._serchId == key).First();
            if (_haveItemList.Where(x => x._itemData._serchId == target._serchId).FirstOrDefault() == null)
            {
                _haveItemList.Add(new PartyItemData(target, 0));
            }
            _haveItemList.ForEach(x => {
                if (x._itemData._serchId == target._serchId)
                {
                    x.haveNum += num;
                    x.haveNum = Mathf.Clamp(x.haveNum, 0, 100);
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"item key is not exist :{key}\n" +
                $"{e}");
        }
    }
    public void ChengeItemNum(ItemData data, int num)
    {
        try
        {
            var target = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList.Where(x => x._data._displayName == data._displayName).First();
            if (_haveItemList.Where(x => x._itemData._serchId == target._serchId).FirstOrDefault() == null)
            {
                _haveItemList.Add(new PartyItemData(target, 0));
            }
            _haveItemList.ForEach(x => {
                if (x._itemData._serchId == target._serchId)
                {
                    x.haveNum += num;
                    x.haveNum = Mathf.Clamp(x.haveNum, 0, 100);
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"item key is not exist :{data._displayName}\n" +
                $"{e}");
        }
    }

    public int GetHaveItemNum(string key)
    {
        var target = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList.Where(x => x._serchId == key).First();
        if (_haveItemList.Where(x => x._itemData._serchId == target._serchId).FirstOrDefault() == null)
        {
            return 0;
        }
        var result = 0;
        _haveItemList.ForEach(x => {
            if (x._itemData._serchId == target._serchId)
            {
                result = x.haveNum;
            }
        });
        return result;
    }
    #endregion
}

public class PartyDBData : VariableDBData
{
    [SerializeField] SavedDBData_party _partyData=new SavedDBData_party();
    [SerializeField, NonEditable] List<string> _firstHaveItemKeyList = new List<string>();//space区切り
    [SerializeField, NonEditable] List<string> _firstPosData = new List<string>();

    protected override SavedDBData GetSavedDBData_child()
    {
        return _partyData;
    }

    protected override void UpdateMember_child(TempDBData data)
    {
        _partyData._haveMoney = data.GetData_int("money");

        _firstHaveItemKeyList = data.GetData_list("haveItem");
        
        _firstPosData = data.GetData_list("firstPos");
        _partyData._PostionData = SetFirstPos(_firstPosData);
    }


    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        _partyData._haveItemList = new List<SavedDBData_party.PartyItemData>();
        var db = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList;
        foreach(var st in _firstHaveItemKeyList)
        {
            try
            {
                var input = SplitItemKey(st);
                var itemName = input[0];
                var itemNum = int.Parse(input[1]);
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

    SavedDBData_party.PostionData SetFirstPos(List<string> datalist)
    {
        Vector2 pos=Vector2.zero;
        Player.DIRECTION dir= Player.DIRECTION.NONE;
        string mname="";
        int successcode=0;
        try
        {
            foreach (var data in datalist)
            {
                var datas = data.Split(' ');
                var head = datas[0];
                switch (head)
                {
                    case "pos":
                        var x = float.Parse(datas[1]);
                        var y = float.Parse(datas[2]);
                        pos = new Vector2(x, y);
                        successcode++;
                        break;
                    case "dir":
                        dir = (Player.DIRECTION)Enum.ToObject(typeof(Player.DIRECTION), int.Parse(datas[1]));
                        successcode++;
                        break;
                    case "mapName":
                        mname = datas[1];
                        successcode++;
                        break;
                    default:
                        ThrowErrorLog(null, _fileName, ErrorCode_uncollectName, _serchId, $"firstPos-{head}");
                        break;
                }
            }
            if (successcode == 3) return new SavedDBData_party.PostionData(pos, dir, mname);
            else
            {
                ThrowErrorLog(null, _fileName, ErrorCode_format, _serchId, "firstPos");
                return null;
            }
        }catch(Exception e)
        {
            ThrowErrorLog(e, _fileName, ErrorCode_default, _serchId, "firstPos");
            return null;
        }
    }
}
