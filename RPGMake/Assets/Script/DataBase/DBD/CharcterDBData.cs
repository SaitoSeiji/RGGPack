using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BattleCharData
{
    [SerializeField] public string _name;
    [SerializeField] public int _hp;
    public int _Hp { get { return _hp; } }
    [SerializeField] public int _attack;
    [SerializeField] public int _guard;
    [SerializeField] public Sprite _charImage;
    public List<SkillDBData> _mySkillList = new List<SkillDBData>();

    public BattleCharData()
    {

    }

    public BattleCharData(int hp, int attack, int guard)
    {
        _hp = hp;
        _attack = attack;
        _guard = guard;
    }

    public BattleCharData(BattleCharData data)
    {
        _name = data._name;
        _hp = data._hp;
        _attack = data._attack;
        _guard = data._guard;
        _mySkillList = data._mySkillList;
        _charImage = data._charImage;
    }

    public BattleCharData Clone()
    {
        return new BattleCharData(this);
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
        var result = new Dictionary<string, int>();
        result.Add("hp", _charData._Hp);
        result.Add("attack", _charData._attack);
        result.Add("guard", _charData._guard);
        return result;
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        var result= new Dictionary<string, string>();
        result.Add("name", _charData._name);
        return result;
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        var result= new Dictionary<string, List<string>>();
        result.Add("skill", _skillNameSet);
        return result;
    }

    protected override void UpdateMember()
    {
        _charData._hp = _Data._memberSet_int["hp"];
        _charData._attack = _Data._memberSet_int["attack"];
        _charData._guard = _Data._memberSet_int["guard"];

        _charData._name = _Data._memberSet_st["name"];
        _skillNameSet = _Data._memberSet_stList["skill"];
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();

        var db= SaveDataController.Instance.GetDB_static<SkillDB>();
        var dbList = db.GetDataList();
        _charData._mySkillList = new List<SkillDBData>();
        foreach (var skill in _skillNameSet)
        {
            var data = dbList.Where(x => x.name == skill).First();
            _charData._mySkillList.Add(data as SkillDBData);
        }
        EditorUtility.SetDirty(this);
    }
}
