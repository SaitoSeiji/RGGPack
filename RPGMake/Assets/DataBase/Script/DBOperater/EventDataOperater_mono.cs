using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EventDataOperater_mono : MonoBehaviour
{
    [SerializeField,HideInInspector]public TextAsset _textAsset;
    [SerializeField] string _txtname;
    [SerializeField] EventDB eventDb;

    //[ContextMenu("createTest")]
    //void CreateTest()
    //{
    //    string path = DBIO.CreateSavePath_txt(_txtname);
    //    string txt = DBIO.ReadText(path);
    //    var dataList =  EventDataOperater.GetConverted(txt);
    //    EventDataOperater.GetLog(dataList);
    //}

    [ContextMenu("SyncDatabyTxt")]
    public void SyncDatabyTxt()
    {
        string path = DBIO.CreateSavePath_txt(_txtname);
        var txt = DBIO.TrimType( DBIO.ReadText(path));
        EventDataOperater.SyncDataByTxt(eventDb,txt.replaced,_txtname);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EventDataOperater_mono))]
    public class EventDataOperater_mono_editor:Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var script = target as EventDataOperater_mono;

            EditorGUI.BeginChangeCheck();
            script._textAsset = EditorGUILayout.ObjectField(
                "テキストデータ", script._textAsset, typeof(TextAsset), true)
                as TextAsset;
            if (EditorGUI.EndChangeCheck()&& script._textAsset!=null)
            {
                script._txtname = script._textAsset.name;
            }
        }
    }
#endif
}
