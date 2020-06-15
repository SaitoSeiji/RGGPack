using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "SkillDB", menuName = "DataBases/DataBase/SkillDB", order = 0)]
public class SkillDB : StaticDB
{
    [SerializeField] public List<SkillDBData> _dataList;
    
    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }

    protected override void SetDataList_child(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as SkillDBData).ToList();
    }
}
