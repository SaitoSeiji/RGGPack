using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class SavedDBData_player : SavedDBData_char
{
    [SerializeField,Space] public int _spMax;
    [SerializeField] public int _spNow;
    [SerializeField] public int _hpNow;

    public SavedDBData_player()
    {

    }

    public SavedDBData_player(SavedDBData_player data) : base(data)
    {
        _spNow = data._spNow;
        _spMax = data._spMax;
        _hpNow = data._hpNow;
    }
    public SavedDBData_player(SavedDBData_char data) : base(data)
    {
    }

    public new SavedDBData_player Clone()
    {
        return new SavedDBData_player(this);
    }

    public SavedDBData_player Copy(SavedDBData_char data)
    {
        var result =new SavedDBData_player(data);
        result._spNow = _spNow;
        result._spMax = _spMax;
        result._hpNow = _hpNow;
        return result;
    }
    //ここからデータへの変更をするもの
    public void AddSkill(SkillDBData data)
    {
        if (_mySkillList.Contains(data)) return;
        _mySkillList.Add(data);
    }
}

[CreateAssetMenu(fileName = "PlayerDBData", menuName = "DataBases/Data/PlayerDBData", order = 0)]
public class PlayerDBData : VariableDBData
{
    [SerializeField] SavedDBData_player _charData = new SavedDBData_player();
    [SerializeField, NonEditable] List<string> _skillNameSet = new List<string>();
    
    protected override SavedDBData GetSavedDBData_child()
    {
        return _charData;
    }


    protected override void UpdateMember_child(TempDBData data)
    {
        var temp = (SavedDBData_char)_charData;
        Partial_CharcterDBData.UpdateMember(ref temp, ref _skillNameSet, data);
        _charData = _charData.Copy(temp);
        _charData._spMax = data.GetData_int("spMax");
        _charData._spNow = data.GetData_int("spNow");
        _charData._hpNow = data.GetData_int("hpNow");
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();

        var temp = (SavedDBData_char)_charData;
        Partial_CharcterDBData.RateUpdateMemeber(ref temp, _skillNameSet);
        _charData = _charData.Copy(temp);
        EditorUtility.SetDirty(this);
    }
}
