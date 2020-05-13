using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class SavedDBData_item : SavedDBData
{
    [SerializeField]public string _displayName;
    [SerializeField] public int _maxNum;
    [SerializeField] public int _haveNum;
    [SerializeField, TextArea(0, 10)] public string _explanation;
    [SerializeField]public Sprite _itemImage;
    public override void ModifyData()
    {
        if (_haveNum > _maxNum) _haveNum = _maxNum;
    }
}
[CreateAssetMenu(fileName = "ItemDBD",menuName = "DataBases/Data/ItemDBData",order = 0)]
public class ItemDBData : VariableDBData
{
    [SerializeField] SavedDBData_item _data;

    protected override SavedDBData GetSavedDBData_child()
    {
        return _data;
    }
    protected override void UpdateMember_child(TempDBData data)
    {
        _data._displayName = data.GetData_st("displayName");
        _data._maxNum = data.GetData_int("maxNum");
        _data._haveNum = data.GetData_int("haveNum");
        _data._explanation = data.GetData_st("explanation");
    }
}
