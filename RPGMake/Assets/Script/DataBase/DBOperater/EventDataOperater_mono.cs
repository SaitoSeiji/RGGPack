using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDataOperater_mono : MonoBehaviour
{
    [SerializeField,HideInInspector] string _txtname;
    [SerializeField] EventDB eventDb;
    

    [ContextMenu("SyncDatabyTxt")]
    public void SyncDatabyTxt()
    {
        string path = DBIO.CreateSavePath_txt(_txtname);
        var txt = DBIO.TrimType( DBIO.ReadText(path));
        EventDataOperater.SyncDataByTxt(eventDb,txt.replaced,_txtname);
    }

    public void SetReadFileName(string fileName)
    {
        _txtname = fileName;
    }
    
}
