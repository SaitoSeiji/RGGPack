using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EquipDB", menuName = "DataBases/DataBase/EquipDB", order = 0)]
public class EquipDB : StaticDB
{
    [SerializeField] public List<EquipDBData> _dataList=new List<EquipDBData>();

    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }

    protected override void SetDataList_child(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as EquipDBData).ToList();
    }
}
