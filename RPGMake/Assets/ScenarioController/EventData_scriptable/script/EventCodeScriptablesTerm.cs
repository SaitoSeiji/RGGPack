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
        return true;
        //if (_termList == null || _termList.Count == 0) return true;

        //foreach (var coalTerm in _termList)
        //{
        //    if (!SaveDataHolder.Instance.CheckCollectFlagName(coalTerm._FlagName))
        //    {
        //        Debug.Log("uncorrect flag Name : " + " : " + coalTerm._FlagName);
        //    }
        //    if (coalTerm._FlagCount >= 0)
        //    {
        //        if (_orMode)
        //        {
        //            foreach (var saveTerm in SaveDataHolder.Instance.flagSaveData)
        //            {

        //                if (saveTerm._FlagName == coalTerm._FlagName
        //                    && saveTerm._FlagCount == coalTerm._FlagCount)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var saveTerm in SaveDataHolder.Instance.flagSaveData)
        //            {

        //                if (saveTerm._FlagName == coalTerm._FlagName
        //                    && saveTerm._FlagCount != coalTerm._FlagCount)
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //}
        //if (_orMode)
        //{
        //    return false;
        //}
        //else
        //{
        //    return true;
        //}
    }
}

