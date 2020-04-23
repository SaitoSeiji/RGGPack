using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DBOperater_mono : MonoBehaviour
{
    [SerializeField,HideInInspector] string _fileName;
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
            case "Skill":
                return typeof(SkillDB);
            default:
                return typeof(AbstractDB);
        }
    }
    #endregion

    AbstractDB GetDB(System.Type type)
    {
        return _dataBaseList.Where(x => x.GetType() == type).FirstOrDefault();
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
    public void SyncDBByTxt()
    {
        var read = ReadFile(_fileName);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
            op.SyncDataByTxt(ReadFile(_fileName).text);
        }
        else if (type == typeof(FlagDB))
        {
            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
            op.SyncDataByTxt(ReadFile(_fileName).text);
        }
        else if (type == typeof(SkillDB))
        {
            var op = new DBOperater<SkillDBData, SkillDB>(db as SkillDB, _fileName);
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

    public void SetReadFileName(string fileName)
    {
        _fileName = fileName;
    }
}
