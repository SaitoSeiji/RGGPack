using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;


[System.Serializable]
public class EnemySetData
{
    public List<CharcterDBData> _charList;
}


[CreateAssetMenu(fileName = "EnemySetDBData", menuName = "DataBases/Data/EnemySetDBData", order = 0)]
public class EnemySetDBData : StaticDBData
{
    public EnemySetData _enemySetData=new EnemySetData();

    [SerializeField, NonEditable] List<string> _enemyNameList = new List<string>();

    protected override void UpdateMember_child(TempDBData data)
    {
        _enemyNameList = data.GetData_list("enemy");
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        var db = SaveDataController.Instance.GetDB_static<CharcterDB>()._dataList;
        _enemySetData._charList = new List<CharcterDBData>();
        foreach (var names in _enemyNameList)
        {
            try
            {
                var data = db.Where(x => x.name == names).First();
                _enemySetData._charList.Add(data as CharcterDBData);
            }
            catch(InvalidOperationException e)
            {
                ThrowErrorLog(e, names,ErrorCode_uncollectName);
            }
        }
        EditorUtility.SetDirty(this);
    }
}
