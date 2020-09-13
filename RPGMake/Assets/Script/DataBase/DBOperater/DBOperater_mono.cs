using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
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

    IDBOerater GetDBOperator(string type)
    {
        switch (type)
        {
            case "Player"  :return new DBOperater<PlayerDBData  , PlayerDB  >(GetDB<PlayerDB  >());
            case "Flag"    :return new DBOperater<FlagDBData    , FlagDB    >(GetDB<FlagDB    >());
            case "Party"   :return new DBOperater<PartyDBData   , PartyDB   >(GetDB<PartyDB   >());
            case "Item"    :return new DBOperater<ItemDBData    , ItemDB    >(GetDB<ItemDB    >());
            case "Shop"    :return new DBOperater<ShopDBData    , ShopDB    >(GetDB<ShopDB    >());
            case "Skill"   :return new DBOperater<SkillDBData   , SkillDB   >(GetDB<SkillDB   >());
            case "Charcter":return new DBOperater<CharcterDBData, CharcterDB>(GetDB<CharcterDB>());
            case "EnemySet":return new DBOperater<EnemySetDBData, EnemySetDB>(GetDB<EnemySetDB>());
            case "Equip"   :return new DBOperater<EquipDBData   , EquipDB   >(GetDB<EquipDB   >());
            default: return null;
        }
    }
    string GetDirPath()
    {
        return (istest)? "Test":"Product";
    }

    T GetDB<T>()
        where T : AbstractDB
    {
        return _dataBaseList.Where(x => x is T).FirstOrDefault() as T;
    }
    [ContextMenu("SyncDataByTxt")]
    public void SyncDBByTxt()
    {
        var read = DBIO.TrimTypeText(_readFile.text);
        GetDBOperator(read.type).SyncDataByTxt(_readFile,GetDirPath());
        
    }

    public void RateUpdate()
    {
        var read = DBIO.TrimTypeText(_readFile.text);
        GetDBOperator(read.type).RateUpdate();
        
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
