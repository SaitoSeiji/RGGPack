using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    public void AddTerm(string id,int data, DataMemberInspector.DATATYPE datatype,DataMemberInspector.HIKAKU hikaku)
    {
        //内容
        var target= _termList.Where(x => x._id == id).FirstOrDefault();
        if (target == null)
        {
            try
            {
                target = new DataMemberInspector(id, datatype);
            }catch(ArgumentException e)
            {
                throw e;
            }
        }
        target.AddData( data, hikaku);
        _termList.Add(target);
    }

    //そのうち分離and設定しやすくする
    bool CheckSatisfyTerm()
    {
        if (_termList == null || _termList.Count == 0) return true;
        foreach(var coalTerm in _termList)
        {
            bool unitResult = false;
            switch (coalTerm._datatype)
            {
                case DataMemberInspector.DATATYPE.FLAG:
                    unitResult= CheckSatisfy_flag(coalTerm);
                    break;
                case DataMemberInspector.DATATYPE.ITEM:
                    unitResult = CheckSatisfy_item(coalTerm);
                    break;
            }
            if (_orMode && unitResult) return true;//1つでも条件を満たしたらtrue
            else if (!_orMode && !unitResult) return false;//1つでも条件を満たさなかったらfalse
        }
        if (_orMode) return false;//ここに行くのは1つも条件を満たさなかった場合
        else return true;//ここに行くのはすべての条件を満たした場合
        //var db_flag = SaveDataController.Instance.GetDB_var<FlagDB, SavedDBData_flag>();
        //var db_item = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
        //foreach (var coalTerm in _termList)
        //{
        //    //var checkData = SaveDataController.Instance.GetData<FlagDB>(coalTerm);
        //    var check = db_flag.Where(x => x._serchId == coalTerm._id).FirstOrDefault();
        //    int check2=0;
        //    if (check == null)
        //    {
        //        check2 = db_item.GetHaveItemNum(coalTerm._id);
        //    }
        //    if (check == null&&check2==0) return false;

        //    int checkData = (check != null) ? check.flagNum : check2;

        //    int checkedData = coalTerm._memberSet[0].data;
        //    if (_orMode)
        //    {
        //        switch (coalTerm._memberSet[0]._hikaku)
        //        {
        //            case DataMemberInspector.HIKAKU.EQUAL:
        //                if (checkData == checkedData) return true;
        //                break;
        //            case DataMemberInspector.HIKAKU.LESS:
        //                if (checkData < checkedData) return true;
        //                break;
        //            case DataMemberInspector.HIKAKU.MORE:
        //                if (checkData> checkedData) return true;
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch (coalTerm._memberSet[0]._hikaku)
        //        {
        //            case DataMemberInspector.HIKAKU.EQUAL:
        //                if (checkData != checkedData) return false;
        //                break;
        //            case DataMemberInspector.HIKAKU.LESS:
        //                if (checkData > checkedData) return false;
        //                break;
        //            case DataMemberInspector.HIKAKU.MORE:
        //                if (checkData < checkedData) return false;
        //                break;
        //        }
        //    }
        //}

        //return !_orMode;
    }

    bool CheckSatisfy_flag(DataMemberInspector term)
    {
        var db_flag = SaveDataController.Instance.GetDB_var<FlagDB, SavedDBData_flag>();
        var survayData = db_flag.Where(x => x._serchId == term._id).First().flagNum;
        foreach(var t in term._memberSet)
        {
            var staticData = t.data;
            switch (t._hikaku)
            {
                case DataMemberInspector.HIKAKU.EQUAL:
                    if (!(survayData == staticData)) return false;
                    break;
                case DataMemberInspector.HIKAKU.LESS:
                    if (!(survayData < staticData)) return false;
                    break;
                case DataMemberInspector.HIKAKU.MORE:
                    if (!(survayData > staticData)) return false;
                    break;
            }
        }
        return true;
    }
    bool CheckSatisfy_item(DataMemberInspector term)
    {
        var db_item = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
        var survayData = db_item.GetHaveItemNum(term._id);
        foreach (var t in term._memberSet)
        {
            var staticData = t.data;
            switch (t._hikaku)
            {
                case DataMemberInspector.HIKAKU.EQUAL:
                    if (!(survayData == staticData)) return false;
                    break;
                case DataMemberInspector.HIKAKU.LESS:
                    if (!(survayData < staticData)) return false;
                    break;
                case DataMemberInspector.HIKAKU.MORE:
                    if (!(survayData > staticData)) return false;
                    break;
            }
        }
        return true;
    }


}

