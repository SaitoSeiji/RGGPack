using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventCodeScriptablesTerm
{
    [SerializeField]List<DataMemberInspector> _termList;
    [SerializeField] bool _orMode = false;//trueだと１つでも条件を満たしていたらtrue

    public bool CoalEnable()
    {
        return CheckSatisfyTerm();
    }

    //そのうち分離and設定しやすくする
    bool CheckSatisfyTerm()
    {
        if (_termList == null || _termList.Count == 0) return true;

        foreach (var coalTerm in _termList)
        {
            var checkData = SaveDataController.Instance.GetData<FlagDB>(coalTerm);
            if (checkData < 0) checkData = SaveDataController.Instance.GetData<ItemDB>(coalTerm);
            if (checkData < 0) return false;

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

