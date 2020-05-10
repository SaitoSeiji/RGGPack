using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDB", menuName = "DataBases/DataBase/ItemDB", order = 0)]
public class ItemDB : VariableDB
{
    [SerializeField] public List<ItemDBData> _dataList;

    public override AbstractDBData FindData_id(string id)
    {
        return FindData_id(_dataList, id);
    }

    public override void InitData()
    {
        InitData(_dataList);
    }


    public override List<AbstractDBData> GetDataList()
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }
    public override void SetDataList(List<AbstractDBData> list)
    {
        _dataList = list.Select(x => x as ItemDBData).ToList();
    }
}
