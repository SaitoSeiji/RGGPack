using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                return HIKAKU.NONE;
        }
    }
    [System.Serializable]
    public class StSet
    {
        [SerializeField] public string memberName;
        [SerializeField] public int data;
        [SerializeField] public HIKAKU _hikaku;
        public StSet(string name,int num,HIKAKU hikaku)
        {
            memberName = name;
            data = num;
            _hikaku = hikaku;
        }
    }

    [SerializeField]public string _id;
    [SerializeField]public List<StSet> _memberSet=new List<StSet>();
    
    public DataMemberInspector(string id)
    {
        _id = id;
    }

    public void AddData(string mName, int data, HIKAKU hikaku)
    {
        _memberSet.Add(new StSet(mName, data, hikaku));
    }
    public void AddData(string mName, int data)
    {
        _memberSet.Add(new StSet(mName, data, HIKAKU.NONE));
    }

    public void ResetData()
    {
        _memberSet = new List<StSet>();
    }
}
