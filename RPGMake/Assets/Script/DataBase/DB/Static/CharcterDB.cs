using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ChracterDB", menuName = "DataBases/DataBase/ChracterDB", order = 0)]
public class CharcterDB : StaticDB
{
    [SerializeField] public List<CharcterDBData> _dataList;

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
        _dataList = list.Select(x => x as CharcterDBData).ToList();
    }
    //public override void SetDataList(List<AbstractDBData> list)
    //{
    //    
    //}
}
