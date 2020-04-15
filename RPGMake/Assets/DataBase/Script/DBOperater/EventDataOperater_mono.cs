using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDataOperater_mono : MonoBehaviour
{
    [SerializeField] string txtname;
    [SerializeField] EventDB eventDb;

    [ContextMenu("createTest")]
    void CreateTest()
    {
        string path = DBIO.CreateSavePath_txt(txtname);
        string txt = DBIO.ReadText(path);
        var dataList =  EventDataOperater.GetConverted(txt);
        EventDataOperater.GetLog(dataList);
    }

    [ContextMenu("SyncDatabyTxt")]
    void SyncDatabyTxt()
    {
        string path = DBIO.CreateSavePath_txt(txtname);
        string txt = DBIO.ReadText(path);
        EventDataOperater.SyncDataByTxt(eventDb,txt);
    }
}
