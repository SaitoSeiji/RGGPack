using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySetDB", menuName = "DataBases/DataBase/EnemySetDB", order = 0)]
public class EnemySetDB : StaticDB
{
    [SerializeField] List<AbstractDBData> dataList=new List<AbstractDBData>();

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
