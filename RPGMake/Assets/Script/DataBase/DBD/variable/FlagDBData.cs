using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedDBData_flag : SavedDBData
{
    public int flagNum;
}

[CreateAssetMenu(fileName = "FlagDBD",menuName = "DataBases/Data/FlagData",order = 0)]
public class FlagDBData : VariableDBData
{
    [SerializeField] SavedDBData_flag _data;

    public override SavedDBData GetSavedDBData()
    {
        return _data;
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        var result= new Dictionary<string, string>();
        return result;
    }

    protected override Dictionary<string, int> InitMember_int()
    {
        var result = new Dictionary<string, int>();
        result.Add("flagNum",0);
        return result;
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        return new Dictionary<string, List<string>>();
    }

    public override void UpdateMember(TempDBData data)
    {
        _data._serchId = data._serchId;
        _data.flagNum = data.GetData_int("flagNum");
    }

    public static DataMemberInspector SetFlagNum(string id,int Num)
    {
        var result = new DataMemberInspector(id);
        result.AddData("flagNum", Num);
        return result;
    }
}
