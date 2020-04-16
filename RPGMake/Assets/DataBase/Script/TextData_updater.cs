using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(DBOperater_mono), typeof(EventDataOperater_mono))]
public class TextData_updater : MonoBehaviour
{
    [SerializeField,HideInInspector] DBOperater_mono _dbOperater;
    [SerializeField,HideInInspector] EventDataOperater_mono _eventOperater;
    
    [SerializeField]List<TextAsset> _dataBaseText = new List<TextAsset>();
    [SerializeField] TextAsset _eventDataText;

    public void DataUpdate_ev()
    {
        _eventOperater.SetReadFileName(_eventDataText.name);
        _eventOperater.SyncDatabyTxt();
    }

    public void DataUpdate_db()
    {
        foreach (var data in _dataBaseText)
        {
            _dbOperater.SetReadFileName(data.name);
            _dbOperater.SyncDBTxt();
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
                script.DataUpdate_db();
                script.DataUpdate_ev();
            }
        }
    }
#endif
}
