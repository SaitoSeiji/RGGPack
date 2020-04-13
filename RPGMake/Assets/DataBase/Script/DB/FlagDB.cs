using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
  fileName = "FlagDB",
  menuName = "DataBases/DataBase/FlagData",
  order = 0)
]
public class FlagDB : AbstractDB
{
    [SerializeField]public List<AbstractDBData> _dataList;

    public override AbstractDBData FindData_id(string id)
    {
        return FindData_id<AbstractDBData>(_dataList, id);
    }

    public override void InitData()
    {
        InitData(_dataList);
    }
    public override List<AbstractDBData> GetDataList()
    {
        return _dataList;
    }

}
