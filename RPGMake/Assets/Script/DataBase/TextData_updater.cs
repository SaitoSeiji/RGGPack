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

    [SerializeField] DBOperratorSetting _settingData;
    List<TextAsset> _dataBaseText { get { return _settingData._DataBaseTextList; } }
    TextAsset _eventDataText { get { return _settingData._EventDataText; } }


    void DataUpdate_ev()
    {
        _eventOperater.SetReadFile(_eventDataText);
        _eventOperater.SyncDatabyTxt();
    }

    void DataUpdate_db()
    {
        foreach (var data in _dataBaseText)
        {
            _dbOperater.SetReadFile(data);
            _dbOperater.SyncDBByTxt();
        }
        SaveDataController.Instance.InitSaveDataList();
        foreach (var data in _dataBaseText)
        {
            _dbOperater.SetReadFile(data);
            _dbOperater.RateUpdate();
        }
        SaveDataController.Instance.SaveAction(false);
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

    void UpdateSetting()
    {
        _dbOperater.SetSettingObject(_settingData);
        _eventOperater.SetSettingObject(_settingData);
        GameController_setting.SetUpGameController(_settingData);
    }
    
    [ContextMenu("resetDB")]
    void ResetDB()
    {
        _settingData.ResetDataBase();
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
            }else if (GUILayout.Button("updateSetting"))
            {
                script.Init();
                script.UpdateSetting();
            }
        }
    }
#endif
}
