using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "DBOperratorSetting", menuName = "Setting/DBOperratorSetting", order = 0)]
public class DBOperratorSetting : ScriptableObject,IEnable_initDB
{

    [SerializeField] bool _isTest;
    public bool _IsTest { get { return _isTest; } }
    #region text
    [SerializeField] List<TextAsset> _dataBaseText = new List<TextAsset>();
    public List<TextAsset> _DataBaseTextList { get { return _dataBaseText; } }
    [SerializeField] TextAsset _eventDataText;
    public TextAsset _EventDataText { get { return _eventDataText; } }
    #endregion
    #region db
    public List<StaticDB> _DataBaseList_static { get { return _dataBaseList.Where(x => x is StaticDB).Select(x=>(StaticDB)x).ToList(); } }
    public List<VariableDB> _DataBaseList_variable { get { return _dataBaseList.Where(x => x is VariableDB).Select(x => (VariableDB)x).ToList(); } }

    [SerializeField] List<AbstractDB> _dataBaseList = new List<AbstractDB>();
    public List<AbstractDB> _DataBaseList { get { return _dataBaseList; } }
    [SerializeField] EventDB _eventDb;
    public EventDB _EventDb { get { return _eventDb; } }
    #endregion
    #region resource
    [SerializeField,Space(10)] AudioDataBase _audioDB;
    public AudioDataBase _AudioDB { get { return _audioDB; } }
    [SerializeField] SpriteDataBase _imageDB;
    public SpriteDataBase _ImageDB { get { return _imageDB; } }
    [SerializeField] MapDataBase _mapDB;
    public MapDataBase _MapDB { get { return _mapDB; } }
    #endregion

#if UNITY_EDITOR
    public void ResetDataBase()
    {
        _dataBaseList.ForEach(x =>
        {
            var list= x.GetDataList(this);
            list.ForEach(data => {
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(data));
            });
            x.SetDataList(new List<AbstractDBData>(), this);
        });
        _eventDb._scriptableList.ForEach(data => {
            AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(data));
        });
        _eventDb._scriptableList = new List<EventCodeScriptable>();
    }
#endif
}
