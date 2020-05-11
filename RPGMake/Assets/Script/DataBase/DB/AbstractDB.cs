using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IEnable_initDB { }

public abstract class AbstractDB : ScriptableObject
{
    //後で治す：AbstractDBをジェネリックにしたくなかったために、dataBaseのListがどのデータでも入るようになってしまっている
    //不具合が出次第修正
    public abstract List<AbstractDBData> GetDataList(IEnable_initDB enable);
    public abstract void SetDataList(List<AbstractDBData> list,IEnable_initDB enable);

    //protected static T FindData_id<T>(List<T> dataList, string id)
    //    where T : AbstractDBData
    //{
    //    return dataList.Where(x => x._Data._serchId == id).FirstOrDefault();
    //}

    //public abstract AbstractDBData FindData_id(string id);
    //public abstract void InitData();

    //protected static void InitData<T>(List<T> dataList)
    //    where T : AbstractDBData
    //{
    //    foreach(var data in dataList)
    //    {
    //        data.InitData();
    //    }
    //}

    //public string CreateDataTxt()
    //{
    //    var list = GetDataList();
    //    string result = "";
    //    foreach (var data in list)
    //    {
    //        result += data.CreateSaveTxt();
    //    }
    //    result += "end";
    //    return result;
    //}
    
    //今のところデータの変更が行われたときに呼ばれる
    public static void DataUpdateAction<T>(TempDBData dbData,List<T> list)
        where T:AbstractDBData
    {
        foreach(var data in list)
        {
            if (data.name == dbData._serchId)
            {
                data.DataUpdateAction(dbData);
                break;
            }
        }
    }
}
