using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ItemDB", menuName = "DataBases/DataBase/ItemDB", order = 0)]
public class ItemDB : StaticDB
{
    [SerializeField]public List<ItemDBData> _dataList;
    
    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }

    public override void SetDataList(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as ItemDBData).ToList();
    }
}
