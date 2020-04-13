using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBOperater_mono: MonoBehaviour
{
    public enum DBTYPE
    {
        FLAG,ITEM
    }
    [SerializeField] DBTYPE dbType;

    [SerializeField]string fileName;
    [SerializeField] AbstractDB _dataBase;

    [SerializeField,Space(10)] string oldName;
    [SerializeField] DBData _data;

    [ContextMenu("add Data")]
    public void AddDB()
    {
        switch (dbType)
        {
            case DBTYPE.FLAG:
                {
                    var op = new DBOperater<FlagDBData, FlagDB>(_dataBase as FlagDB, _dataBase.SavePath);
                    op.AddDBD(fileName);
                }
                break;
            case DBTYPE.ITEM:
                {
                    var op = new DBOperater<ItemDBData, ItemDB>(_dataBase as ItemDB, _dataBase.SavePath);
                    op.AddDBD(fileName);
                }
                break;
        }
    }

    [ContextMenu("remove data")]
    public void RemoveDB()
    {
        switch (dbType)
        {
            case DBTYPE.FLAG:
                {
                    var op = new DBOperater<FlagDBData, FlagDB>(_dataBase as FlagDB, _dataBase.SavePath);
                    op.RemoveDBD(fileName);
                }
                break;
            case DBTYPE.ITEM:
                {
                    var op = new DBOperater<ItemDBData, ItemDB>(_dataBase as ItemDB, _dataBase.SavePath);
                    op.RemoveDBD(fileName);
                }
                break;
        }
    }
    [ContextMenu("Edit data")]
    public void EditDB()
    {
        switch (dbType)
        {
            case DBTYPE.FLAG:
                {
                    //現状名前しか編集できない
                    //inspectorにdiectionaryの情報を表示したい
                    var op = new DBOperater<FlagDBData, FlagDB>(_dataBase as FlagDB, _dataBase.SavePath);
                    if (op == null) return;
                    _data = op.EditDBD(fileName);
                    oldName = fileName;
                }
                break;
            case DBTYPE.ITEM:
                {
                    var op = new DBOperater<ItemDBData, ItemDB>(_dataBase as ItemDB, _dataBase.SavePath);
                    if (op == null) return;
                    _data = op.EditDBD(fileName);
                    oldName = fileName;
                }
                break;
        }

    }
    [ContextMenu("Update data")]
    public void UpdateDB()
    {
        switch (dbType)
        {
            case DBTYPE.FLAG:
                {
                    var op = new DBOperater<FlagDBData, FlagDB>(_dataBase as FlagDB, _dataBase.SavePath);
                    op.UpdateDBD(_data, oldName);
                }
                break;
            case DBTYPE.ITEM:
                {
                    var op = new DBOperater<ItemDBData, ItemDB>(_dataBase as ItemDB, _dataBase.SavePath);
                    op.UpdateDBD(_data, oldName);
                }
                break;
        }
    }
    [ContextMenu("SyncDataByTxt")]
    public void SyncDBTxt()
    {
        switch (dbType)
        {
            case DBTYPE.FLAG:
                {
                    var op = new DBOperater<FlagDBData, FlagDB>(_dataBase as FlagDB, _dataBase.SavePath);
                    op.SyncDataByTxt();
                }
                break;
            case DBTYPE.ITEM:
                {
                    var op = new DBOperater<ItemDBData, ItemDB>(_dataBase as ItemDB, _dataBase.SavePath);
                    op.SyncDataByTxt();
                }
                break;
        }
    }
    [ContextMenu("SyncTxtByData")]
    public void SyncTxtDB()
    {
        switch (dbType)
        {
            case DBTYPE.FLAG:
                {
                    var op = new DBOperater<FlagDBData, FlagDB>(_dataBase as FlagDB, _dataBase.SavePath+"_sync");
                    op.SyncTxtByData();
                }
                break;
            case DBTYPE.ITEM:
                {
                    var op = new DBOperater<ItemDBData, ItemDB>(_dataBase as ItemDB, _dataBase.SavePath + "_sync");
                    op.SyncTxtByData();
                }
                break;
        }
    }
    
}
