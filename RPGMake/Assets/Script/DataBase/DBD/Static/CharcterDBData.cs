using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

[System.Serializable]
public class SavedDBData_char:SavedDBData
{
    [SerializeField] string _name;
    public string _Name { get { return _name; } }
    [SerializeField] public int _hpMax;
    public int _HpMax { get { return _hpMax; } }
    [SerializeField] public int _attack;
    [SerializeField] public int _guard;
    [SerializeField] public int _money;
    [SerializeField] public int _exp;
    [SerializeField] public Sprite _charImage;
    public List<SkillDBData> _mySkillList = new List<SkillDBData>();//重複を許さないのでハッシュセットにできるならいい　でもinspectorでいじれない？

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

    //public virtual SavedDBData_char Clone()
    //{
    //    return new SavedDBData_char(this);
    //}

    public void SetName(string name)
    {
        _name = name;
    }
}

public static class Partial_CharcterDBData
{
    public static void UpdateMember(ref SavedDBData_char charData,ref List<string> skillNameSet, TempDBData dbData)
    {
        charData._hpMax = dbData.GetData_int("hpMax");
        charData._attack = dbData.GetData_int("attack");
        charData._guard = dbData.GetData_int("guard");

        charData.SetName( dbData.GetData_st("name"));
        charData._money = dbData.GetData_int("money");
        charData._exp = dbData.GetData_int("exp");
        skillNameSet = dbData.GetData_list("skill");

    }

    public static void RateUpdateMemeber(ref SavedDBData_char charData, List<string> skillNameSet)
    {
        var db = SaveDataController.Instance.GetDB_static<SkillDB>();
        charData._mySkillList = new List<SkillDBData>();
        foreach (var skill in skillNameSet)
        {
            try
            {
                var data = db._dataList.Where(x => x.name == skill).First();
                charData._mySkillList.Add(data);
            }
            catch(Exception e)
            {
                Debug.LogError($"charName is {charData._serchId}\n" +
                    $"skillName is {skill}:\n" +
                    $"{e}");
            }
        }
    }
}

[CreateAssetMenu(fileName = "CharcterDBData", menuName = "DataBases/Data/CharcterDBData", order = 0)]
public class CharcterDBData : StaticDBData
{
    [SerializeField]public SavedDBData_char _charData=new SavedDBData_char();

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
