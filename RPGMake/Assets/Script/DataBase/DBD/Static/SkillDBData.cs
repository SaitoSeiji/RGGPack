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
    [SerializeField] SkillCommandData _data=new SkillCommandData();
    public SkillCommandData _Data { get { return _data; } }

    protected override void UpdateMember_child(TempDBData data)
    {
        _data._skillName = data.GetData_st("skillName");
        _data._rowRate = data.GetData_int("rate");
        _data._target = (Battle_targetDicide.TargetType)Enum.ToObject(typeof(Battle_targetDicide.TargetType), data.GetData_int("target"));
    }
}
