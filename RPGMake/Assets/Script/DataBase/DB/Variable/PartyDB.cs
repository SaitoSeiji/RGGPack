using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PartyDB", menuName = "DataBases/DataBase/PartyDB", order = 0)]
public class PartyDB : VariableDB
{
    [SerializeField] List<PartyDBData> _dataList=new List<PartyDBData>();

    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }
    public override void SetDataList(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as PartyDBData).ToList();
    }
}
