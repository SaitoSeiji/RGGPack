using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SavedDBData_item : SavedDBData
{
    [SerializeField]public string _displayName;
    [SerializeField] public int _maxNum;
    [SerializeField] public int _haveNum;
    [SerializeField, TextArea(0, 10)] public string _explanation;
}

//現状haveNumがマイナスになる
[CreateAssetMenu(fileName = "ItemDBD",menuName = "DataBases/Data/ItemDBData",order = 0)]
public class ItemDBData : VariableDBData
{
    [SerializeField] SavedDBData_item _data;

    public override SavedDBData GetSavedDBData()
    {
        return _data;
    }

    protected override Dictionary<string, int> InitMember_int()
    {
        var result= new Dictionary<string, int>();
        result.Add("maxNum", 0);
        result.Add("haveNum", 0);
        return result;
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        var result = new Dictionary<string, string>();
        result.Add("displayName", "");
        result.Add("explanation", "");
        return result;
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        return new Dictionary<string, List<string>>();
    }

    public override void UpdateMember(TempDBData data)
    {
        _data._serchId = data._serchId;
        _data._displayName = data.GetData_st("displayName");
        _data._maxNum = data.GetData_int("maxNum");
        _data._haveNum = data.GetData_int("haveNum");
        _data._explanation = data.GetData_st("explanation");
    }

    public static DataMemberInspector AddHaveNum(string id,int num)
    {
        var result = new DataMemberInspector(id);

        //var have= SaveDataController.Instance.GetData<ItemDB>(id, "haveNum");
        var db = SaveDataController.Instance.GetDB_var<ItemDB, SavedDBData_item>();
        var have= db.Where(x=>x._serchId==id).First()  ;
        result.AddData( "haveNum", have._haveNum+num);
        return result;
    }
    
    public override void DataUpdateAction(TempDBData data)
    {
        //var maxNum = data._memberSet_int["maxNum"];
        //var haveNum = data._memberSet_int["haveNum"];
        //if (haveNum < 0)
        //{
        //    haveNum = 0;
        //}
        //if (haveNum > maxNum)
        //{
        //    haveNum = maxNum;
        //}
        //data._memberSet_int["haveNum"] = haveNum;
    }
}
