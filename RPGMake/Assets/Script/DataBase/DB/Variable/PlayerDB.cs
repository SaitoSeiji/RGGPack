﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerDB", menuName = "DataBases/DataBase/PlayerDB", order = 0)]
public class PlayerDB : VariableDB
{
    [SerializeField] List<PlayerDBData> _dataList;
    
    public override List<AbstractDBData> GetDataList(IEnable_initDB enable)
    {
        return _dataList.Select(x => (AbstractDBData)x).ToList();
    }
    public override void SetDataList(List<AbstractDBData> list, IEnable_initDB enable)
    {
        _dataList = list.Select(x => x as PlayerDBData).ToList();
    }
}