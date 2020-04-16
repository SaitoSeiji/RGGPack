using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DBOperater_mono : MonoBehaviour
{
    [SerializeField,HideInInspector] string _fileName;
    [SerializeField,HideInInspector]public TextAsset _txtFile;
    [SerializeField] List<AbstractDB> _dataBaseList=new List<AbstractDB>();
    //現在一時利用停止中
    [SerializeField, Space(10),HideInInspector] string oldName;
    [SerializeField, HideInInspector] DBData _data;
    #region static
    static (string type,string text) ReadFile(string fileName)
    {
        string path = DBIO.CreateSavePath_txt(fileName);
        string txt= DBIO.ReadText(path);
        return DBIO.TrimType(txt);
    }
    static System.Type JudgeDBType(string  type)
    {
        switch (type)
        {
            case "Item":
                return typeof(ItemDB);
            case "Flag":
                return typeof(FlagDB);
            default:
                return typeof(AbstractDB);
        }
    }
    #endregion

    AbstractDB GetDB(System.Type type)
    {
        return _dataBaseList.Where(x => x.GetType() == type).FirstOrDefault();
    }
    
    void SetFileName()
    {
        _fileName = _txtFile.name;
    }
    //[ContextMenu("add Data")]
    public void AddDB()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            op.AddDBD(_fileName);
        }
        else if (type == typeof(FlagDB))
        {

            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            op.AddDBD(_fileName);
        }
    }
    //[ContextMenu("remove data")]
    public void RemoveDB()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            op.RemoveDBD(_fileName);
        }
        else if (type == typeof(FlagDB))
        {

            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            op.RemoveDBD(_fileName);
        }
    }
    //[ContextMenu("Edit data")]
    public void EditDB()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            if (op == null) return;
            _data = op.EditDBD(_fileName);
            oldName = _fileName;
        }
        else if (type == typeof(FlagDB))
        {

            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            if (op == null) return;
            _data = op.EditDBD(_fileName);
            oldName = _fileName;
        }

    }
    //[ContextMenu("Update data")]
    public void UpdateDB()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            op.UpdateDBD(_data, oldName);
        }
        else if (type == typeof(FlagDB))
        {

            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            op.UpdateDBD(_data, oldName);
        }
    }
    [ContextMenu("SyncDataByTxt")]
    public void SyncDBTxt()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            op.SyncDataByTxt(ReadFile(_fileName).text);
        }
        else if(type == typeof(FlagDB))
        {

            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            op.SyncDataByTxt(ReadFile(_fileName).text);
        }
    }
    //[ContextMenu("SyncTxtByData")]
    public void SyncTxtDB()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            op.SyncTxtByData();
        }
        else if (type == typeof(FlagDB))
        {

            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            op.SyncTxtByData();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DBOperater_mono))]
    public class DBOperater_mono_editor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as DBOperater_mono;
            EditorGUI.BeginChangeCheck();
            script._txtFile = EditorGUILayout.ObjectField(
                "テキストデータ", script._txtFile, typeof(TextAsset), true)
                as TextAsset;
            if (EditorGUI.EndChangeCheck()&&script._txtFile!=null)
            {
                script._fileName = script._txtFile.name;
            }
            DrawDefaultInspector();
        }

    }

#endif
}
