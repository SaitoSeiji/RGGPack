using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ChracterDB", menuName = "DataBases/DataBase/ChracterDB", order = 0)]
public class CharcterDB : StaticDB
{
    [SerializeField] public List<CharcterDBData> _dataList=new List<CharcterDBData>();
    
    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }
    protected override void SetDataList_child(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as CharcterDBData).ToList();
    }
}
