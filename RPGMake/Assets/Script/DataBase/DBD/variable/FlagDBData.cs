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

    protected override SavedDBData GetSavedDBData_child()
    {
        return _data;
    }
    
    protected override void UpdateMember_child(TempDBData data)
    {
        _data.flagNum = data.GetData_int("flagNum");
    }
}
