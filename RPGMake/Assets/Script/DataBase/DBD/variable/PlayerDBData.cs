using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class SavedDBData_player : SavedDBData_char
{
    [SerializeField,Space] public int _spMax;
    [SerializeField] public int _spNow;
    [SerializeField] public int _hpNow;

    public SavedDBData_player()
    {

    }

    public SavedDBData_player(SavedDBData_player data) : base(data)
    {
        _spNow = data._spNow;
        _spMax = data._spMax;
        _hpNow = data._hpNow;
    }
    public SavedDBData_player(SavedDBData_char data) : base(data)
    {
    }

    public new SavedDBData_player Clone()
    {
        return new SavedDBData_player(this);
    }

    public SavedDBData_player Copy(SavedDBData_char data)
    {
        var result =new SavedDBData_player(data);
        result._spNow = _spNow;
        result._spMax = _spMax;
        result._hpNow = _hpNow;
        return result;
    }
}

[CreateAssetMenu(fileName = "PlayerDBData", menuName = "DataBases/Data/PlayerDBData", order = 0)]
public class PlayerDBData : VariableDBData
{
    [SerializeField] SavedDBData_player _charData = new SavedDBData_player();

    public override SavedDBData GetSavedDBData()
    {
        return _charData;
    }

    [SerializeField, NonEditable] List<string> _skillNameSet = new List<string>();
    #region init
    protected override Dictionary<string, int> InitMember_int()
    {
        var result= Partial_CharcterDBData.InitMember_int(_charData);
        result.Add("spMax", _charData._spMax);
        result.Add("spNow", _charData._spNow);
        result.Add("hpNow", _charData._hpNow);
        return result;
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        return Partial_CharcterDBData.InitMember_st(_charData);
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        return Partial_CharcterDBData.InitMemeber_stList(_skillNameSet);
    }

    public override void UpdateMember(TempDBData data)
    {
        var temp = (SavedDBData_char)_charData;
        Partial_CharcterDBData.UpdateMember(ref temp, ref _skillNameSet, data);
        _charData = _charData.Copy(temp);
        _charData._spMax = data.GetData_int("spMax");
        _charData._spNow = data.GetData_int("spNow");
        _charData._hpNow = data.GetData_int("hpNow");
        _charData._serchId = data._serchId;
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();

        var temp = (SavedDBData_char)_charData;
        Partial_CharcterDBData.RateUpdateMemeber(ref temp, _skillNameSet);
        _charData = _charData.Copy(temp);
        EditorUtility.SetDirty(this);
    }
    #endregion

    //public static SavedDBData_player ConvertDBData2plData(TempDBData data)
    //{
    //    var temp = Partial_CharcterDBData.ConvertDBData2BattleCharData(data);
    //    var result = new SavedDBData_player();
    //    result = result.Copy(temp);
    //    result._spMax = data._memberSet_int["spMax"];
    //    result._spNow = data._memberSet_int["spNow"];
    //    result._hpNow = data._memberSet_int["hpNow"];
    //    return result;
    //}

    //public static TempDBData ConvertplData2DBData(SavedDBData_player plData, TempDBData beforeDBData)
    //{
    //    var temp = beforeDBData;
    //    temp._memberSet_int["spNow"] = plData._spNow;
    //    temp._memberSet_int["hpNow"] = plData._hpNow;
    //    return temp;
    //}
}
