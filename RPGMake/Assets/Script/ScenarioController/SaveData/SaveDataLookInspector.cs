using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
//Inspectorからセーブデータを確認及び変更する
public class SaveDataLookInspector : MonoBehaviour
{
    [SerializeField]List<SavedDBData_player> _savedata_pl;
    [SerializeField]List<SavedDBData_flag> _savedata_flag;
    [SerializeField]SavedDBData_party _savedata_party;
    [SerializeField] Dictionary<string, List<SavedDBData>> _saveDataList = new Dictionary<string, List<SavedDBData>>();//<dbName,dataList>
    bool synced = false;
    [ContextMenu("sync")]
    void Sync_from()
    {
        synced = true;
        _savedata_pl = SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>();
        _savedata_flag = SaveDataController.Instance.GetDB_var<FlagDB, SavedDBData_flag>();
        _savedata_party = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
    }

    void Sync_to()
    {
        if (!synced) return;
        SaveDataController.Instance.SetSaveDataList_editorOnly<PlayerDB>(_savedata_pl.Select(x=>x as SavedDBData).ToList());
        SaveDataController.Instance.SetSaveDataList_editorOnly<FlagDB>(_savedata_flag.Select(x=>x as SavedDBData).ToList());
        SaveDataController.Instance.SetData<PartyDB, SavedDBData_party>(_savedata_party);
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(SaveDataLookInspector))]
    public class SaveDataLookInspectorEditor:Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var scr = target as SaveDataLookInspector;
            if (GUILayout.Button("syncFrom"))
            {
                scr.Sync_from();
            }
            if (GUILayout.Button("syncTo"))
            {
                scr.Sync_to();
            }
        }
    }
    #endif
}
