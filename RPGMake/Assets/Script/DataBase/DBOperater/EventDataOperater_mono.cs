using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDataOperater_mono : MonoBehaviour
{
    [SerializeField, HideInInspector] TextAsset _readText;
    [SerializeField] bool isTest;
    [SerializeField] EventDB eventDb;
    

    [ContextMenu("SyncDatabyTxt")]
    public void SyncDatabyTxt()
    {
        var txt = DBIO.TrimType(_readText.text);
        var path =(isTest)? $"Test/{_readText.name}":$"Product/{_readText.name}";
        EventDataOperater.SyncDataByTxt(eventDb, txt.replaced,path );
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
