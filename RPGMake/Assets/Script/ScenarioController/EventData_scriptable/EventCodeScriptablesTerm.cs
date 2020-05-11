using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class EventCodeScriptablesTerm
{
    [SerializeField]public List<DataMemberInspector> _termList=new List<DataMemberInspector>();
    [SerializeField]public bool _orMode = false;//trueだと１つでも条件を満たしていたらtrue

    public bool CoalEnable()
    {
        return CheckSatisfyTerm();
    }

    public void ResetTerm()
    {
        _termList = new List<DataMemberInspector>();
    }

    public void AddTerm(string id,string member,int data,DataMemberInspector.HIKAKU hikaku)
    {
        var target= _termList.Where(x => x._id == id).FirstOrDefault();
        if (target == null)
        {
            target = new DataMemberInspector(id);
        }
        target.AddData(member, data, hikaku);
        _termList.Add(target);
    }

    //そのうち分離and設定しやすくする
    bool CheckSatisfyTerm()
    {
        if (_termList == null || _termList.Count == 0) return true;
        var db_flag = SaveDataController.Instance.GetDB_var<FlagDB, SavedDBData_flag>();
        var db_item = SaveDataController.Instance.GetDB_var<ItemDB, SavedDBData_item>();
        foreach (var coalTerm in _termList)
        {
            //var checkData = SaveDataController.Instance.GetData<FlagDB>(coalTerm);
            var check = db_flag.Where(x => x._serchId == coalTerm._id).FirstOrDefault();
            SavedDBData_item check2=null;
            if (check == null)
            {
                check2 = db_item.Where(x => x._serchId == coalTerm._id).FirstOrDefault();
            }
            if (check == null&&check2==null) return false;

            int checkData = (check != null) ? check.flagNum : check2._haveNum;

            int checkedData = coalTerm._memberSet[0].data;
            if (_orMode)
            {
                switch (coalTerm._memberSet[0]._hikaku)
                {
                    case DataMemberInspector.HIKAKU.EQUAL:
                        if (checkData == checkedData) return true;
                        break;
                    case DataMemberInspector.HIKAKU.LESS:
                        if (checkData < checkedData) return true;
                        break;
                    case DataMemberInspector.HIKAKU.MORE:
                        if (checkData> checkedData) return true;
                        break;
                }
            }
            else
            {
                switch (coalTerm._memberSet[0]._hikaku)
                {
                    case DataMemberInspector.HIKAKU.EQUAL:
                        if (checkData != checkedData) return false;
                        break;
                    case DataMemberInspector.HIKAKU.LESS:
                        if (checkData > checkedData) return false;
                        break;
                    case DataMemberInspector.HIKAKU.MORE:
                        if (checkData < checkedData) return false;
                        break;
                }
            }
        }

        return !_orMode;
    }
}

