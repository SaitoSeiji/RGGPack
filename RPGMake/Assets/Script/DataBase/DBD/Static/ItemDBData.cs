using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using RPGEnums;
using System;
using DBDInterface;

[System.Serializable]
public class ItemData:ICommandData
{
    [SerializeField]public string _displayName;
    [SerializeField, TextArea(0, 10)] public string _explanation;
    [SerializeField] public TargetType _targetType;
    [SerializeField] public ResourceType _targetResource;
    [SerializeField] public int _effectNum;
    [SerializeField] public int _price;

    [SerializeField]public Sprite _itemImage;
    

    CommandData ICommandData.GetCommandData()
    {
        var result= new CommandData();
        result._target = _targetType;
        result._targetResourceType = _targetResource;
        result._effectNum = _effectNum;

        result._useResourceType = ResourceType.NONE;
        result._useNum = 0;
        return result;
    }

}
[CreateAssetMenu(fileName = "ItemDBD",menuName = "DataBases/Data/ItemDBData",order = 0)]
public class ItemDBData : StaticDBData,IShopContent
{
    [SerializeField] public ItemData _data;
    
    protected override void UpdateMember_child(TempDBData data)
    {
        _data._displayName = data.GetData_st("displayName");
        _data._explanation = data.GetData_st("explanation");
        //_data._useScene = data.GetData_int("useScene");
        _data._targetType = (TargetType)Enum.ToObject(typeof(TargetType), data.GetData_int("targetType"));
        _data._targetResource = (ResourceType)Enum.ToObject(typeof(ResourceType), data.GetData_int("targetResource"));
        _data._effectNum = data.GetData_int("effectNum");
        _data._price = data.GetData_int("price");
    }


    (string id, string name, int price, Sprite image) IShopContent.GetShopContentData()
    {
        return (_serchId, _data._displayName, _data._price, _data._itemImage);
    }
}
