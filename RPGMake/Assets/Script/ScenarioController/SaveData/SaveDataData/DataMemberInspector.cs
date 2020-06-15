using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class DataMemberInspector
{
    public enum HIKAKU
    {
        NONE,EQUAL,LESS,MORE
    }
    public static HIKAKU CreateHikaku(string key)
    {
        switch (key)
        {
            case "equal":
                return HIKAKU.EQUAL;
            case "less":
                return HIKAKU.LESS;
            case "more":
                return HIKAKU.MORE;
            default:
                throw new ArgumentException();
        }
    }

    public enum DATATYPE
    {
        FLAG,ITEM
    }
    public static DATATYPE CreateDataType(string key)
    {
        switch (key)
        {
            case "flag":
                return DATATYPE.FLAG;
            case "item":
                return DATATYPE.ITEM;
            default:
                throw new ArgumentException();
        }
    }
    [System.Serializable]
    public class StSet
    {
        [SerializeField] public int data;
        [SerializeField] public HIKAKU _hikaku;
        public StSet(int num,HIKAKU hikaku)
        {
            data = num;
            _hikaku = hikaku;
        }
    }

    [SerializeField]public string _id;
    [SerializeField] public DATATYPE _datatype; 
    [SerializeField]public List<StSet> _memberSet=new List<StSet>();//複数持つのはa<x<bを表現するため
    
    public DataMemberInspector(string id,DATATYPE datatype)
    {
        _datatype = datatype;
        _id = id;

        bool error=false;
        switch (datatype)
        {
            case DATATYPE.FLAG:
                {
                    var db = SaveDataController.Instance.GetDB_var<FlagDB, SavedDBData_flag>();
                    error = (db.Where(x => x._serchId == id).FirstOrDefault() == null);
                    break;
                }
            case DATATYPE.ITEM:
                {
                    var db = SaveDataController.Instance.GetDB_static<ItemDB>()._dataList;
                    error = (db.Where(x => x._serchId == id).FirstOrDefault() == null);
                    break;
                }
        }
        if (error) throw new ArgumentException();
    }

    public void AddData( int data, HIKAKU hikaku)
    {
        _memberSet.Add(new StSet(data, hikaku));
    }
    public void AddData(int data)
    {
        _memberSet.Add(new StSet( data, HIKAKU.NONE));
    }

    public void ResetData()
    {
        _memberSet = new List<StSet>();
    }
}
