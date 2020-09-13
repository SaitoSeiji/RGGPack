using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



[System.Serializable]
public class EquipData
{
    //装備する位置
    //防具が複数になったり、武器の種類のデータが欲しくなったら修正が必要
    public enum EquipPosition
    {
        WEPON=0,
        GUARD=1,
        //ACCESSARY=2//現状攻撃と防御しかないので装備する意味がないのでカット
    }
    [SerializeField]public EquipPosition _equipPosition;

    //アイテムとほぼ一緒なのでどうにかしたい
    [SerializeField] public string _displayName;
    [SerializeField, TextArea(0, 10)] public string _explanation;
    [SerializeField] public int _effectNum;
    [SerializeField] public int _price;
    [SerializeField] public Sprite _itemImage;
}

[CreateAssetMenu(fileName = "EquipDBData", menuName = "DataBases/Data/EquipDBData", order = 0)]
public class EquipDBData : StaticDBData,IShopContent
{
    public EquipData _data;

    protected override void UpdateMember_child(TempDBData data)
    {
        _data._displayName = data.GetData_st("displayName");
        _data._explanation = data.GetData_st("explanation");
        _data._effectNum = data.GetData_int("effectNum");
        _data._price = data.GetData_int("price");
        _data._equipPosition = (EquipData.EquipPosition)Enum.ToObject(typeof(EquipData.EquipPosition), data.GetData_int("equipPosition"));
    }

    (string id, string name, int price, Sprite image) IShopContent.GetShopContentData()
    {
        return (_serchId, _data._displayName, _data._price, _data._itemImage);
    }
}
