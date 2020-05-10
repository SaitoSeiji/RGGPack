using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class PlayerCharData : BattleCharData
{
    [SerializeField,Space] public int _spMax;
    [SerializeField] public int _spNow;
    [SerializeField] public int _hpNow;

    public PlayerCharData()
    {

    }

    public PlayerCharData(PlayerCharData data) : base(data)
    {
        _spNow = data._spNow;
        _spMax = data._spMax;
        _hpNow = data._hpNow;
    }
    public PlayerCharData(BattleCharData data) : base(data)
    {
    }

    public override BattleCharData Clone()
    {
        return new PlayerCharData(this);
    }

    public PlayerCharData Copy(BattleCharData data)
    {
        var result =new PlayerCharData(data);
        result._spNow = _spNow;
        result._spMax = _spMax;
        result._hpNow = _hpNow;
        return result;
    }
}

[CreateAssetMenu(fileName = "PlayerDBData", menuName = "DataBases/Data/PlayerDBData", order = 0)]
public class PlayerDBData : AbstractDBData
{
    [SerializeField] PlayerCharData _charData = new PlayerCharData();
    public PlayerCharData _CharData { get { return (PlayerCharData)_charData.Clone(); } }

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

    protected override void UpdateMember()
    {
        var temp = (BattleCharData)_charData;
        Partial_CharcterDBData.UpdateMember(ref temp,ref _skillNameSet, _Data);
        _charData =_charData.Copy(temp);
        _charData._spMax = _Data._memberSet_int["spMax"];
        _charData._spNow = _Data._memberSet_int["spNow"];
        _charData._hpNow = _Data._memberSet_int["hpNow"];
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();

        var temp = (BattleCharData)_charData;
        Partial_CharcterDBData.RateUpdateMemeber(ref temp, _skillNameSet);
        _charData = _charData.Copy(temp);
        EditorUtility.SetDirty(this);
    }
    #endregion

    public static PlayerCharData ConvertDBData2plData(DBData data)
    {
        var temp = Partial_CharcterDBData.ConvertDBData2BattleCharData(data);
        var result = new PlayerCharData();
        result = result.Copy(temp);
        result._spMax = data._memberSet_int["spMax"];
        result._spNow = data._memberSet_int["spNow"];
        result._hpNow = data._memberSet_int["hpNow"];
        return result;
    }

    public static DBData ConvertplData2DBData(PlayerCharData plData, DBData beforeDBData)
    {
        var temp = beforeDBData;
        temp._memberSet_int["spNow"] = plData._spNow;
        temp._memberSet_int["hpNow"] = plData._hpNow;
        return temp;
    }
}
