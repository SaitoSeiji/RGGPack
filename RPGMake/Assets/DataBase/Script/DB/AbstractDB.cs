using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class AbstractDB : ScriptableObject
{
    [SerializeField] string savePath;//txtの名前とasset/DB下のファイル名（存在しない場合はエラー）
    public string SavePath { get { return savePath; } }

    //後で治す：AbstractDBをジェネリックにしたくなかったために、dataBaseのListがどのデータでも入るようになってしまっている
    //不具合が出次第修正
    public abstract List<AbstractDBData> GetDataList();

    protected static T FindData_id<T>(List<T> dataList, string id)
        where T : AbstractDBData
    {
        return dataList.Where(x => x._Data._serchId == id).FirstOrDefault();
    }

    public abstract AbstractDBData FindData_id(string id);
    public abstract void InitData();

    protected static void InitData<T>(List<T> dataList)
        where T : AbstractDBData
    {
        foreach(var data in dataList)
        {
            data.InitData();
        }
    }

    public string CreateDataTxt()
    {
        var list = GetDataList();
        string result = "";
        foreach (var data in list)
        {
            result += data.CreateSaveTxt();
        }
        result += "end";
        return result;
    }
    
    //今のところデータの変更が行われたときに呼ばれる
    public void DataUpdateAction(DBData dbData)
    {
        var list = GetDataList();
        foreach(var data in list)
        {
            if (data._Data._serchId == dbData._serchId)
            {
                data.DataUpdateAction(dbData);
                break;
            }
        }
    }
}
