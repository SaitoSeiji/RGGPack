using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class  VariableDB : AbstractDB,IEnable_initDB
{
    //データの変更が発生するクラス
    //jsonに書き込みを行う

    public List<SavedDBData> GetSavedDataList()
    {
        var list = GetDataList(this).Select(x=>(VariableDBData)x);
        var result = new List<SavedDBData>();
        foreach (var data in list)
        {
            result.Add(data.GetSavedDBData());
        }

        return result;
    }
    
}
