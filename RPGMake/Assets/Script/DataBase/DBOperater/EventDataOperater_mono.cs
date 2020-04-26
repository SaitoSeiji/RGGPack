using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDataOperater_mono : MonoBehaviour
{
    [SerializeField, HideInInspector] TextAsset _readText;
    [SerializeField] EventDB eventDb;
    

    [ContextMenu("SyncDatabyTxt")]
    public void SyncDatabyTxt()
    {
        var txt = DBIO.TrimType(_readText.text);
        EventDataOperater.SyncDataByTxt(eventDb, txt.replaced, _readText.name);
    }

    //public void SetReadFileName(string fileName)
    //{
    //    _txtname = fileName;
    //}
    public void SetReadFile(TextAsset text)
    {
        _readText = text;
    }
}
