using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class EventDataMonoBehaviour : MonoBehaviour
{
    [SerializeField,HideInInspector]EventCodeScriptable _eventData;
    [SerializeField] string _eventDataId;

    private void Start()
    {
        SetEventData();
    }

    public void EventAction()
    {
        EventCodeReadController.Instance.SetEventData(_eventData);
        EventCodeReadController.Instance.StartEvent();
    }

    public bool CheckCoalEnable()
    {
        if (_eventData == null) return false;
        return _eventData.CoalEnable();
    }

    //調べられるかどうかの更新処理
    bool FindEnable()
    {
        return CheckCoalEnable();
    }

    //editor====================
    void SetEventID()
    {
        _eventDataId = _eventData.name;
    }

    bool SetEventData()
    {
        _eventData = EventController.Instance.GetEventData(_eventDataId);
        return _eventData != null;
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(EventDataMonoBehaviour))]
    public class EventDataMonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //eventDataを更新したときに参照切れを起こす問題の対策

            //データを登録したら自動でstringで保存する
            DrawDefaultInspector();
            var script = target as EventDataMonoBehaviour;
            EditorGUI.BeginChangeCheck();
            script._eventData = EditorGUILayout.ObjectField(
                "イベントデータ", script._eventData, typeof(EventCodeScriptable), true)
                as EventCodeScriptable;
            if (EditorGUI.EndChangeCheck())
            {
                script.SetEventID();
                EditorUtility.SetDirty(script);
            }

            //参照が切れた後にidが変わっていなければ参照をつけなおす
            if (!EditorApplication.isPlaying)
            {
                if (!string.IsNullOrEmpty(script._eventDataId)
                    && !script._eventDataId.Contains("chenged:")
                    && script._eventData == null)
                {
                    if (!script.SetEventData())
                    {
                        script._eventDataId = "chenged:" + script._eventDataId;
                        EditorUtility.SetDirty(script);
                    }
                }
            }
        }
    }
#endif
}


