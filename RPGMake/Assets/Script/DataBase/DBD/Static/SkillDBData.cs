using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPGEnums;

[System.Serializable]
public class SkillCommandData
{
    [SerializeField] public string _skillName;
    [SerializeField] public TargetType _target;//効果対象

    [SerializeField] public ResourceType _useResourceType;//使用するリソース(hpなど)
    [SerializeField] public int _useNum;//使用量
    [SerializeField] ResourceType _targetResourceType;//ダメージ（回復）を行う対象のリソース
    
    [SerializeField] float _attackRate;//効果量(攻撃力にかけて使う)
    public float _AttackRate//intで受け取って100で割って代入
    {
        get{return _attackRate;}
        set{ _attackRate = value / 100f; }
    }
    public ResourceType _TargetResourceType
    {
        get { return _targetResourceType; }
        set
        {
            var add = value;
            if (add == ResourceType.NONE) add = ResourceType.HP;//デフォルト値をhpに設定
            _targetResourceType = add;
        }
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
        _data._target = (TargetType)Enum.ToObject(typeof(TargetType)
            , data.GetData_int("target"));
        _data._useResourceType= (ResourceType)Enum.ToObject(typeof(ResourceType)
            , data.GetData_int("useResource"));
        _data._useNum = data.GetData_int("useNum");
        _data._TargetResourceType= (ResourceType)Enum.ToObject(typeof(ResourceType)
            , data.GetData_int("targetResource"));
        _data._AttackRate = data.GetData_int("rate");
    }
}
