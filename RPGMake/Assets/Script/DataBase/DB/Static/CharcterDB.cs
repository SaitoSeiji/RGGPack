using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChracterDB", menuName = "DataBases/DataBase/ChracterDB", order = 0)]
public class CharcterDB : StaticDB
{
    [SerializeField] List<AbstractDBData> dataList;

    public override AbstractDBData FindData_id(string id)
    {
        return FindData_id(dataList, id);
    }

    public override void InitData()
    {
        InitData(dataList);
    }

    public override List<AbstractDBData> GetDataList()
    {
        return dataList;
    }
}
