using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FlagDBD",menuName = "DataBases/Data/FlagData",order = 0)]
public class FlagDBData : AbstractDBData
{
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

    protected override void UpdateMember()
    {
    }

    public static DataMemberInspector SetFlagNum(string id,int Num)
    {
        var result = new DataMemberInspector(id);
        result.AddData("flagNum", Num);
        return result;
    }
}
