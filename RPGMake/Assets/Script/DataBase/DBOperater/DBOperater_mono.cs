using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DBOperater_mono : MonoBehaviour
{
    [SerializeField] DBOperratorSetting _mySettting;
    [SerializeField, HideInInspector] TextAsset _readFile;
    List<AbstractDB> _dataBaseList { get { return _mySettting._DataBaseList; } }
    //現在一時利用停止中
    [SerializeField, Space(10),HideInInspector] string oldName;
    [SerializeField, HideInInspector] TempDBData _data;
     bool istest { get { return _mySettting._IsTest; } }
    #region static
    static System.Type JudgeDBType(string  type)
    {
        switch (type)
        {
            case "Player":
                return typeof(PlayerDB);
            case "Flag":
                return typeof(FlagDB);
            case "Party":
                return typeof(PartyDB);
            case "Item":
                return typeof(ItemDB);
            case "Shop":
                return typeof(ShopDB);
            case "Skill":
                return typeof(SkillDB);
            case "Charcter":
                return typeof(CharcterDB);
            case "EnemySet":
                return typeof(EnemySetDB);
            default:
                return null;
        }
    }
    #endregion

    string GetDirPath()
    {
        return (istest)? "Test":"Product";
    }

    AbstractDB GetDB(System.Type type)
    {
        return _dataBaseList.Where(x => x.GetType() == type).FirstOrDefault();
    }
    //[ContextMenu("add Data")]
    //public void AddDB()
    //{
    //    var read = ReadFile(_fileName);
    //    var type = JudgeDBType(read.type);
    //    var db = GetDB(type);
    //    if (type == typeof(ItemDB))
    //    {
    //        var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
    //        op.AddDBD(_fileName);
    //    }
    //    else if (type == typeof(FlagDB))
    //    {

    //        var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
    //        op.AddDBD(_fileName);
    //    }
    //}
    ////[ContextMenu("remove data")]
    //public void RemoveDB()
    //{
    //    var read = ReadFile(_fileName);
    //    var type = JudgeDBType(read.type);
    //    var db = GetDB(type);
    //    if (type == typeof(ItemDB))
    //    {
    //        var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
    //        op.RemoveDBD(_fileName);
    //    }
    //    else if (type == typeof(FlagDB))
    //    {

    //        var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
    //        op.RemoveDBD(_fileName);
    //    }
    //}
    ////[ContextMenu("Edit data")]
    //public void EditDB()
    //{
    //    var read = ReadFile(_fileName);
    //    var type = JudgeDBType(read.type);
    //    var db = GetDB(type);
    //    if (type == typeof(ItemDB))
    //    {
    //        var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
    //        if (op == null) return;
    //        _data = op.EditDBD(_fileName);
    //        oldName = _fileName;
    //    }
    //    else if (type == typeof(FlagDB))
    //    {

    //        var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
    //        if (op == null) return;
    //        _data = op.EditDBD(_fileName);
    //        oldName = _fileName;
    //    }
    //}
    ////[ContextMenu("Update data")]
    //public void UpdateDB()
    //{
    //    var read = ReadFile(_fileName);
    //    var type = JudgeDBType(read.type);
    //    var db = GetDB(type);
    //    if (type == typeof(ItemDB))
    //    {
    //        var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
    //        op.UpdateDBD(_data, oldName);
    //    }
    //    else if (type == typeof(FlagDB))
    //    {

    //        var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
    //        op.UpdateDBD(_data, oldName);
    //    }
    //}
    [ContextMenu("SyncDataByTxt")]
    public void SyncDBByTxt()
    {
        var read = DBIO.TrimType(_readFile.text);//ReadFile(_readFile);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
        else if (type == typeof(ShopDB))
        {
            var op = new DBOperater<ShopDBData, ShopDB>(db as ShopDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
        else if (type == typeof(FlagDB))
        {
            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
        else if (type == typeof(SkillDB))
        {
            var op = new DBOperater<SkillDBData, SkillDB>(db as SkillDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
        else if (type == typeof(CharcterDB))
        {
            var op = new DBOperater<CharcterDBData, CharcterDB>(db as CharcterDB);
            op.SyncDataByTxt(_readFile,GetDirPath());
        }
        else if (type == typeof(PlayerDB))
        {
            var op = new DBOperater<PlayerDBData, PlayerDB>(db as PlayerDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
        else if (type == typeof(PartyDB))
        {
            var op = new DBOperater<PartyDBData, PartyDB>(db as PartyDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
        else if (type == typeof(EnemySetDB))
        {
            var op = new DBOperater<EnemySetDBData, EnemySetDB>(db as EnemySetDB);
            op.SyncDataByTxt(_readFile, GetDirPath());
        }
    }

    public void RateUpdate()
    {
        var read = DBIO.TrimType(_readFile.text);//ReadFile(_readFile);
        var type = JudgeDBType(read.type);
        var db = GetDB(type);
        if (type == typeof(ItemDB))
        {
            var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB);
            op.RateUpdate();
        }
        else if (type == typeof(ShopDB))
        {
            var op = new DBOperater<ShopDBData, ShopDB>(db as ShopDB);
            op.RateUpdate();
        }
        else if (type == typeof(FlagDB))
        {
            var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB);
            op.RateUpdate();
        }
        else if (type == typeof(SkillDB))
        {
            var op = new DBOperater<SkillDBData, SkillDB>(db as SkillDB);
            op.RateUpdate();
        }
        else if (type == typeof(CharcterDB))
        {
            var op = new DBOperater<CharcterDBData, CharcterDB>(db as CharcterDB);
            op.RateUpdate();
        }
        else if (type == typeof(PlayerDB))
        {
            var op = new DBOperater<PlayerDBData, PlayerDB>(db as PlayerDB);
            op.RateUpdate();
        }
        else if (type == typeof(PartyDB))
        {
            var op = new DBOperater<PartyDBData, PartyDB>(db as PartyDB);
            op.RateUpdate();
        }
        else if (type == typeof(EnemySetDB))
        {
            var op = new DBOperater<EnemySetDBData, EnemySetDB>(db as EnemySetDB);
            op.RateUpdate();
        }
    }
    //[ContextMenu("SyncTxtByData")]
    //public void SyncTxtDB()
    //{
    //    var read = ReadFile(_fileName);
    //    var type = JudgeDBType(read.type);
    //    var db = GetDB(type);
    //    if (type == typeof(ItemDB))
    //    {
    //        var op = new DBOperater<ItemDBData, ItemDB>(db as ItemDB, _fileName);
    //        op.SyncTxtByData();
    //    }
    //    else if (type == typeof(FlagDB))
    //    {

    //        var op = new DBOperater<FlagDBData, FlagDB>(db as FlagDB, _fileName);
    //        op.SyncTxtByData();
    //    }
    //}

    public void SetReadFile(TextAsset text)
    {
        _readFile = text;
    }
#if UNITY_EDITOR
    public void SetSettingObject(DBOperratorSetting setting)
    {
        _mySettting = setting;
        Debug.Log($"{gameObject.name}:setting updated");
        EditorUtility.SetDirty(this);
    }
#endif
}
