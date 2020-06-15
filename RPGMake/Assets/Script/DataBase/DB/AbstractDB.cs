using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//データをテキストから作成するときにGetDataList等の使用を許可するinterface
public interface IEnable_initDB { }

public abstract class AbstractDB : ScriptableObject
{
    //初期化時のみ使用可能
    //参照の切れたリストが取得できるので、変更をSetDataListで反映する
    public abstract List<AbstractDBData> GetDataList(IEnable_initDB enable);
    protected abstract void SetDataList_child(List<AbstractDBData> list,IEnable_initDB enable);
    public void SetDataList(List<AbstractDBData> list,IEnable_initDB enable)
    {

        SetDataList_child(list, enable);
    }
}
