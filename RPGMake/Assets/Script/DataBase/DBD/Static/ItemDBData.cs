using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RPGEnums;
using System;

[System.Serializable]
public class ItemData
{
    [SerializeField]public string _displayName;
    //[SerializeField] public int _maxNum;
    //[SerializeField] public int _haveNum;
    [SerializeField, TextArea(0, 10)] public string _explanation;
    [SerializeField] public TargetType _targetType;
    [SerializeField] public ResourceType _targetResource;
    [SerializeField] public int _effectNum;

    [SerializeField]public Sprite _itemImage;
    //public override void ModifyData()
    //{
    //    if (_haveNum > _maxNum) _haveNum = _maxNum;
    //}
}
[CreateAssetMenu(fileName = "ItemDBD",menuName = "DataBases/Data/ItemDBData",order = 0)]
public class ItemDBData : StaticDBData
{
    [SerializeField] public ItemData _data;
    
    protected override void UpdateMember_child(TempDBData data)
    {
        _data._displayName = data.GetData_st("displayName");
        _data._explanation = data.GetData_st("explanation");
        //_data._maxNum = data.GetData_int("maxNum");
        //_data._haveNum = data.GetData_int("haveNum");
        //_data._useScene = data.GetData_int("useScene");
        _data._targetType = (TargetType)Enum.ToObject(typeof(TargetType), data.GetData_int("targetType"));
        _data._targetResource = (ResourceType)Enum.ToObject(typeof(ResourceType), data.GetData_int("targetResource"));
        _data._effectNum = data.GetData_int("effectNum");
    }
}
