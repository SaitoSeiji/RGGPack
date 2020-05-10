using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BattleCharData
{
    [SerializeField] public string _name;
    [SerializeField] public int _hpMax;
    public int _HpMax { get { return _hpMax; } }
    [SerializeField] public int _attack;
    [SerializeField] public int _guard;
    [SerializeField] public Sprite _charImage;
    public List<SkillDBData> _mySkillList = new List<SkillDBData>();

    public BattleCharData()
    {

    }

    public BattleCharData(BattleCharData data)
    {
        _name = data._name;
        _hpMax = data._hpMax;
        _attack = data._attack;
        _guard = data._guard;
        _mySkillList = data._mySkillList;
        _charImage = data._charImage;
    }

    public virtual BattleCharData Clone()
    {
        return new BattleCharData(this);
    }
}

public static class Partial_CharcterDBData
{
    public static Dictionary<string, int> InitMember_int(BattleCharData charData)
    {
        var result = new Dictionary<string, int>();
        result.Add("hpMax", charData._HpMax);
        result.Add("attack", charData._attack);
        result.Add("guard", charData._guard);
        return result;
    }
    public static Dictionary<string, string> InitMember_st(BattleCharData charData)
    {
        var result = new Dictionary<string, string>();
        result.Add("name", charData._name);
        return result;
    }

    public static Dictionary<string, List<string>> InitMemeber_stList(List<string> skillNameSet)
    {
        var result = new Dictionary<string, List<string>>();
        result.Add("skill", skillNameSet);
        return result;
    }

    public static void UpdateMember(ref BattleCharData charData,ref List<string> skillNameSet, DBData dbData)
    {
        charData._hpMax = dbData._memberSet_int["hpMax"];
        charData._attack = dbData._memberSet_int["attack"];
        charData._guard = dbData._memberSet_int["guard"];

        charData._name = dbData._memberSet_st["name"];
        skillNameSet = dbData._memberSet_stList["skill"];
    }

    public static void RateUpdateMemeber(ref BattleCharData charData, List<string> skillNameSet)
    {
        var db = SaveDataController.Instance.GetDB_static<SkillDB>();
        charData._mySkillList = new List<SkillDBData>();
        foreach (var skill in skillNameSet)
        {
            var data = db._dataList.Where(x => x.name == skill).First();
            charData._mySkillList.Add(data as SkillDBData);
        }
    }

    //効率かなり悪いのでなんかしたい
    public static BattleCharData ConvertDBData2BattleCharData(DBData dbData)
    {
        //charDataの基本データの登録
        BattleCharData charData = new BattleCharData();
        charData._hpMax = dbData._memberSet_int["hpMax"];
        charData._attack = dbData._memberSet_int["attack"];
        charData._guard = dbData._memberSet_int["guard"];
        charData._name = dbData._memberSet_st["name"];

        //スキルの登録
        var skillNameSet = dbData._memberSet_stList["skill"];
        var db = SaveDataController.Instance.GetDB_static<SkillDB>();
        charData._mySkillList = new List<SkillDBData>();
        foreach (var skill in skillNameSet)
        {
            var data = db._dataList.Where(x => x.name == skill).First();
            charData._mySkillList.Add(data as SkillDBData);
        }

        return charData;
    }
}

[CreateAssetMenu(fileName = "CharcterDBData", menuName = "DataBases/Data/CharcterDBData", order = 0)]
public class CharcterDBData : AbstractDBData
{
    [SerializeField] BattleCharData _charData=new BattleCharData();
    public BattleCharData _CharData { get { return _charData.Clone(); } }

    [SerializeField,NonEditable]List<string> _skillNameSet = new List<string>();

    protected override Dictionary<string, int> InitMember_int()
    {
        return Partial_CharcterDBData.InitMember_int(_charData);
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
        Partial_CharcterDBData.UpdateMember(ref _charData,ref _skillNameSet, _Data);
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        
        Partial_CharcterDBData.RateUpdateMemeber(ref _charData, _skillNameSet);
        EditorUtility.SetDirty(this);
    }
}
