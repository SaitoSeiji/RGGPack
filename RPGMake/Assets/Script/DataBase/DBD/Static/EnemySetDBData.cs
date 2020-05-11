using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


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

    protected override Dictionary<string, int> InitMember_int()
    {
        return new Dictionary<string, int>();
    }

    protected override Dictionary<string, string> InitMember_st()
    {
        return new Dictionary<string, string>();
    }

    protected override Dictionary<string, List<string>> InitMemeber_stList()
    {
        var result= new Dictionary<string, List<string>>();
        result.Add("enemy", _enemyNameList);
        return result = new Dictionary<string, List<string>>();
    }

    public override void UpdateMember(TempDBData data)
    {
        _enemyNameList = data.GetData_list("enemy");
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        var db = SaveDataController.Instance.GetDB_static<CharcterDB>();
        _enemySetData._charList = new List<CharcterDBData>();
        foreach (var skill in _enemyNameList)
        {
            var data = db._dataList.Where(x => x.name == skill).First();
            _enemySetData._charList.Add(data as CharcterDBData);
        }
        EditorUtility.SetDirty(this);
    }
}
