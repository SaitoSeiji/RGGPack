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

    [System.Serializable]
    public class StSet
    {
        [SerializeField] public string memberName;
        [SerializeField] public int data;
        [SerializeField] public HIKAKU _hikaku;
        public StSet(string name,int num)
        {
            memberName = name;
            data = num;
        }
    }

    [SerializeField]public string _id;
    [SerializeField]public List<StSet> _memberSet=new List<StSet>();

    public void AddData(string id,string mName,int data)
    {
        _id = id;
        _memberSet.Add(new StSet(mName,data));
    }

    public void ResetData()
    {
        _memberSet = new List<StSet>();
    }
}
