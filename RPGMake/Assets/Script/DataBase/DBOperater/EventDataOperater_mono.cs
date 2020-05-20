using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EventDataOperater_mono : MonoBehaviour
{
    [SerializeField, HideInInspector] TextAsset _readText;
    [SerializeField] DBOperratorSetting _mySetting;
     bool isTest { get { return _mySetting._IsTest; } }
    public EventDB _EventDb { get { return _mySetting._EventDb; } }
    

    [ContextMenu("SyncDatabyTxt")]
    public void SyncDatabyTxt()
    {
        var txt = DBIO.TrimType(_readText.text);
        var path =(isTest)? $"Test/{_readText.name}":$"Product/{_readText.name}";
        EventDataOperater.SyncDataByTxt(_EventDb, txt.replaced,path );
    }

    //public void SetReadFileName(string fileName)
    //{
    //    _txtname = fileName;
    //}
    public void SetReadFile(TextAsset text)
    {
        _readText = text;
    }

#if UNITY_EDITOR
    public void SetSettingObject(DBOperratorSetting setting)
    {
        _mySetting = setting;
        Debug.Log($"{gameObject.name}:setting updated");
        EditorUtility.SetDirty(this);
    }
#endif
}
