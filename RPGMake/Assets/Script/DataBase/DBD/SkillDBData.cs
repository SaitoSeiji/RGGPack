using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SkillCommandData
{
    public enum TARGET
    {
        NONE, SELF, ENEMY
    }
    [SerializeField] public string _skillName;
    [SerializeField] public TARGET _target;
    [SerializeField] public int _rowRate;
    public float GetRate()
    {
        return _rowRate / 100f;
    }
}

[CreateAssetMenu(fileName = "SkillDBData", menuName = "DataBases/Data/SkillDBData", order = 0)]
public class SkillDBData : AbstractDBData
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

    protected override void UpdateMember()
    {
        _skill._skillName = _Data._memberSet_st["skillName"];
        _skill._rowRate = _Data._memberSet_int["rate"];
        _skill._target = (SkillCommandData.TARGET)Enum.ToObject(typeof(SkillCommandData.TARGET), _Data._memberSet_int["target"]);
    }
}
