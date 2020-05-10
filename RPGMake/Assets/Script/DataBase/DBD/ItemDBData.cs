using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//現状haveNumがマイナスになる
[CreateAssetMenu(fileName = "ItemDBD",menuName = "DataBases/Data/ItemDBData",order = 0)]
public class ItemDBData : AbstractDBData
{
    [SerializeField] string _displayName;
    [SerializeField] int _maxNum;
    int _haveNum;
    [SerializeField, TextArea(0, 10)] string _explanation; 

    protected override Dictionary<string, int> InitMember_int()
    {
        var result= new Dictionary<string, int>();
        result.Add("maxNum", _maxNum);
        result.Add("haveNum", _haveNum);
        return result;
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        var result = new Dictionary<string, string>();
        result.Add("displayName", GetDefaultString(_displayName));
        result.Add("explanation", GetDefaultString(_explanation));
        return result;
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        return new Dictionary<string, List<string>>();
    }

    protected override void UpdateMember()
    {
        _displayName = _Data._memberSet_st["displayName"];
        _maxNum = _Data._memberSet_int["maxNum"];
        _haveNum = _Data._memberSet_int["haveNum"];
        _explanation = _Data._memberSet_st["explanation"];
    }

    public static DataMemberInspector AddHaveNum(string id,int num)
    {
        var result = new DataMemberInspector(id);
        var have= SaveDataController.Instance.GetData<ItemDB>(id, "haveNum");
        result.AddData( "haveNum", have+num);
        return result;
    }
    
    public override void DataUpdateAction(DBData data)
    {
        var maxNum = data._memberSet_int["maxNum"];
        var haveNum = data._memberSet_int["haveNum"];
        if (haveNum < 0)
        {
            haveNum = 0;
        }
        if (haveNum > maxNum)
        {
            haveNum = maxNum;
        }
        data._memberSet_int["haveNum"] = haveNum;
    }
}
