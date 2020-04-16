using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(DBOperater_mono), typeof(EventDataOperater_mono))]
public class TextData_updater : MonoBehaviour
{
    [SerializeField] DBOperater_mono _dbOperater;
    [SerializeField] EventDataOperater_mono _eventOperater;

    [SerializeField, HideInInspector] string _beforeDBText;
    [SerializeField, HideInInspector] string _beforeEventText;
    public void DataUpdate()
    {
        bool chenged = false;
        if (DBChenged())
        {
            _dbOperater.SyncDBTxt();
            SaveDataController.Instance.TestInitSave();
            chenged = true;
        }
        if (EventChenged())
        {
            _eventOperater.SyncDatabyTxt();
            chenged = true;
        }

        if (!chenged)
        {
            Debug.Log("data is not Chenged");
        }
        else
        {
            Debug.Log("data update is end");
        }
    }
    
    void Init()
    {
        if (_dbOperater == null)
        {
            _dbOperater = FindObjectOfType<DBOperater_mono>();
        }
        if (_eventOperater == null)
        {
            _eventOperater = FindObjectOfType<EventDataOperater_mono>();
        }
    }

    bool DBChenged()
    {
        bool result = false;
        string path = DBIO.CreateSavePath_txt(_dbOperater._txtFile.name);
        string text = DBIO.ReadText(path);
        if (!text.Equals(_beforeDBText))
        {
            result = true;
        }
        _beforeDBText = text;
        return result;
    }

    bool EventChenged()
    {
        bool result = false;
        string path = DBIO.CreateSavePath_txt(_eventOperater._textAsset.name);
        string text = DBIO.ReadText(path);
        if (!text.Equals(_beforeEventText))
        {
            result = true;
        }
        _beforeEventText = text;
        return result;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TextData_updater))]
    public class TextData_updater_editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = target as TextData_updater;
            if (GUILayout.Button("UpdateData"))
            {
                script.Init();
                script.DataUpdate();
            }
        }
    }
#endif
}
