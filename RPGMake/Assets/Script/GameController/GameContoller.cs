using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameController_setting
{
#if UNITY_EDITOR
    public static void SetUpGameController(DBOperratorSetting setting)
    {
        EventController.Instance.SetEventDataBase(setting._EventDb);
        SaveDataController.Instance.SetDB_editorOnly(setting._DataBaseList_static, setting._DataBaseList_variable);
        ResourceDB_mono.Instance._audioDB = setting._AudioDB;
        ResourceDB_mono.Instance._imageDB = setting._ImageDB;
        ResourceDB_mono.Instance._mapDB = setting._MapDB;
        EditorUtility.SetDirty(EventController.Instance);
        EditorUtility.SetDirty(SaveDataController.Instance);
        EditorUtility.SetDirty(ResourceDB_mono.Instance);
    }
#endif
}

//仮実装
public class GameContoller : SingletonMonoBehaviour<GameContoller>
{
    [SerializeField] EventDataMonoBehaviour firstEvent;

    public bool _AnyOperate
    {
        get
        {
            bool result = false;
            var evc = EventController.Instance;
            if (evc != null && evc.GetReadNow()) result = true;
            var uic = UIController.Instance;
            if (uic != null && uic._OperateNow) result = true;
            return result;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SaveDataController.Instance.LoadAction();
        Player.Instance.Init();
        if(firstEvent!=null) EventController.Instance.CoalEvent(firstEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
