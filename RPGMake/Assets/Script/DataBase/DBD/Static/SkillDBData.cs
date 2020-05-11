using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SkillCommandData
{
    [SerializeField] public string _skillName;
    [SerializeField] public Battle_targetDicide.TargetType _target;
    [SerializeField] public int _rowRate;
    public float GetRate()
    {
        return _rowRate / 100f;
    }
}

[CreateAssetMenu(fileName = "SkillDBData", menuName = "DataBases/Data/SkillDBData", order = 0)]
public class SkillDBData : StaticDBData
{
    [SerializeField] SkillCommandData _skill=new SkillCommandData();
    public SkillCommandData _SKill { get { return _skill; } }

    protected override Dictionary<string, int> InitMember_int()
    {
        var result = new Dictionary<string, int>();
        result.Add("target", (int)(_skill._target));
        result.Add("rate", _skill._rowRate);
        return result;
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        var result = new Dictionary<string, string>();
        result.Add("skillName", _skill._skillName);
        return result;
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        return new Dictionary<string, List<string>>();
    }

    public override void UpdateMember(TempDBData data)
    {
        _skill._skillName = data.GetData_st("skillName");
        _skill._rowRate = data.GetData_int("rate");
        _skill._target = (Battle_targetDicide.TargetType)Enum.ToObject(typeof(Battle_targetDicide.TargetType), data.GetData_int("target"));
    }
}
