﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySetDB", menuName = "DataBases/DataBase/EnemySetDB", order = 0)]
public class EnemySetDB : StaticDB
{
    [SerializeField] public List<EnemySetDBData> _dataList=new List<EnemySetDBData>();
    
    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }

    protected override void SetDataList_child(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as EnemySetDBData).ToList();
    }
}
