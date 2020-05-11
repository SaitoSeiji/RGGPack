using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SavedDBData_char:SavedDBData
{
    [SerializeField] public string _name;
    [SerializeField] public int _hpMax;
    public int _HpMax { get { return _hpMax; } }
    [SerializeField] public int _attack;
    [SerializeField] public int _guard;
    [SerializeField] public Sprite _charImage;
    public List<SkillDBData> _mySkillList = new List<SkillDBData>();

    public SavedDBData_char()
    {

    }

    public SavedDBData_char(SavedDBData_char data)
    {
        _serchId = data._serchId;
        _name = data._name;
        _hpMax = data._hpMax;
        _attack = data._attack;
        _guard = data._guard;
        _mySkillList = data._mySkillList;
        _charImage = data._charImage;
    }

    public virtual SavedDBData_char Clone()
    {
        return new SavedDBData_char(this);
    }
}

public static class Partial_CharcterDBData
{
    public static void UpdateMember(ref SavedDBData_char charData,ref List<string> skillNameSet, TempDBData dbData)
    {
        charData._hpMax = dbData.GetData_int("hpMax");
        charData._attack = dbData.GetData_int("attack");
        charData._guard = dbData.GetData_int("guard");

        charData._name = dbData.GetData_st("name");
        skillNameSet = dbData.GetData_list("skill");
    }

    public static void RateUpdateMemeber(ref SavedDBData_char charData, List<string> skillNameSet)
    {
        var db = SaveDataController.Instance.GetDB_static<SkillDB>();
        charData._mySkillList = new List<SkillDBData>();
        foreach (var skill in skillNameSet)
        {
            var data = db._dataList.Where(x => x.name == skill).First();
            charData._mySkillList.Add(data);
        }
    }
}

[CreateAssetMenu(fileName = "CharcterDBData", menuName = "DataBases/Data/CharcterDBData", order = 0)]
public class CharcterDBData : StaticDBData
{
    [SerializeField] SavedDBData_char _charData=new SavedDBData_char();
    public SavedDBData_char _CharData { get { return _charData.Clone(); } }

    [SerializeField,NonEditable]List<string> _skillNameSet = new List<string>();

    protected override void UpdateMember_child(TempDBData data)
    {
        Partial_CharcterDBData.UpdateMember(ref _charData, ref _skillNameSet, data);
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        
        Partial_CharcterDBData.RateUpdateMemeber(ref _charData, _skillNameSet);
        EditorUtility.SetDirty(this);
    }
}
