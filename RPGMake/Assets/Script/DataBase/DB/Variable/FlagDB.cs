using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "FlagDB",menuName = "DataBases/DataBase/FlagData",order = 0)]
public class FlagDB : VariableDB
{
    [SerializeField] public List<FlagDBData> _dataList;

    //public override AbstractDBData FindData_id(string id)
    //{
    //    return FindData_id(_dataList, id);
    //}

    //public override void InitData()
    //{
    //    InitData(_dataList);
    //}

    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }
    public override void SetDataList(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as FlagDBData).ToList();
    }
    //public override void SetDataList(List<AbstractDBData> list)
    //{
    //    
}
