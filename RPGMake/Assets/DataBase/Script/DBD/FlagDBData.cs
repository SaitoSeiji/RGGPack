using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(
  fileName = "FlagDBD",
  menuName = "DataBases/Data/FlagData",
  order = 0)
]
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

    protected override void UpdateMember()
    {
    }

    public static DataMemberInspector SetFlagNum(string id,int Num)
    {
        var result = new DataMemberInspector();
        result.AddData(id,"flagNum", Num);
        return result;
    }
}
