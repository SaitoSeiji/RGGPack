using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "ItemDB",
  menuName = "DataBases/DataBase/ItemDB",
  order = 0)
]
public class ItemDB : VariableDB
{
    [SerializeField]List<AbstractDBData> dataList;

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
