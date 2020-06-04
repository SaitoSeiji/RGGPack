using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

[System.Serializable]
public class NeedExpList
{
    [SerializeField,NonEditable]int _maxlevel;
    [SerializeField, NonEditable] int _firstExp;
    [SerializeField, NonEditable] float _expRate;
    [SerializeField, NonEditable] List<int> _needExpList;//i->i+1に必要な経験値

    public NeedExpList(int maxLevel,int firstExp,float expRate)
    {
        _maxlevel = maxLevel;
        _firstExp = firstExp;
        _expRate = expRate;
        _needExpList=SetNeedExpList();
    }

    public int this[int level]
    {
        get
        {
            if (level < _maxlevel) return _needExpList[level];
            else return 0;
        }
    }

    //checkLevelで必要な経験値
    int GetTargetLevelExp_raw(int checkLevel)
    {
        return (int)(_firstExp * Mathf.Pow(_expRate, checkLevel - 1));
    }
    List<int> SetNeedExpList()
    {
        var result = new List<int>(_maxlevel + 1);
        result.Add(0);
        for (int i = 1; i < result.Capacity; i++)
        {
            result.Add(GetTargetLevelExp_raw(i));
        }
        return result;
    }
}

[System.Serializable]
public class SavedDBData_player : SavedDBData_char
{
    #region field
    [SerializeField,Space] public int _spMax;
    [SerializeField] public int _spNow;
    [SerializeField] public int _hpNow;

    [SerializeField] float _expRate;//経験値の増加量　とりあえず仮でa*ExpRate^levelの指数関数を採用
    public float ExpRate
    {
        get { return _expRate; }
        set { _expRate = value / 1000f; }
    }
    [SerializeField] public int _firstExp;

    [Serializable]
    public class ParamGrowData
    {
        public int hp;
        public int sp;
        public int attack;
        public int guard;
    }
    [SerializeField]public ParamGrowData _paramGrowData=new ParamGrowData();

    [SerializeField] public int _level=1;
    [SerializeField]public NeedExpList _needExpList;
    #endregion
    //ここからデータへの変更をするもの
    public void Init()
    {
        _needExpList = new NeedExpList(99, _firstExp, ExpRate);
    }

    public void AddSkill(SkillDBData data)
    {
        if (_mySkillList.Contains(data)) return;
        _mySkillList.Add(data);
    }

    //レベルの更新と上昇レベルの取得
    public int UpdateLevel()
    {
        int up;
        int templevel = _level;
        for( up =0; true; up++)
        {
            var targetexp = GetTargetLevelExp_sum(templevel);
            if (targetexp < _exp)templevel++;
            else break;
        }
        _level += up;
        UpdateParam(up);
        return up;
    }

    void UpdateParam(int up)
    {
        _hpMax += up * _paramGrowData.hp;
        _spMax += up * _paramGrowData.sp;
        _attack += up * _paramGrowData.attack;
        _guard += up * _paramGrowData.guard;
        if (up > 0)
        {
            _hpNow = _hpMax;
            _spNow = _spMax;
        }
    }
    #region データへの参照をするもの
    //checkLevelで必要な経験値
    public int GetTargetLevelExp_raw(int checkLevel)
    {
        return _needExpList[checkLevel];
    }
    public int GetTargetLevelExp_sum(int checkLevel)
    {
        var result = 0;
        for(int i = 1; i <= checkLevel; i++)
        {
            result += _needExpList[i];
        }
        return result;
    }
    #endregion
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
        _charData._spMax = data.GetData_int("spMax");
        _charData._hpNow = _charData._hpMax;
        _charData._spNow = _charData._spMax;
        _charData.ExpRate = data.GetData_int("expRate");
        _charData._firstExp = data.GetData_int("expFirst");
        _charData._paramGrowData=UpdateMember_growData(data.GetData_list("paramGrow"));
        _charData.Init();
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();

        var temp = (SavedDBData_char)_charData;
        Partial_CharcterDBData.RateUpdateMemeber(ref temp, _skillNameSet);
        EditorUtility.SetDirty(this);
    }
    //growDataについてのupdateMember
    //長くなったので分けた
    static SavedDBData_player.ParamGrowData UpdateMember_growData(List<string> growlist)
    {
        //成長データ
        var result = new SavedDBData_player.ParamGrowData();
        foreach (var growdata in growlist)
        {
            try
            {
                var input = growdata.Split(' ');
                int num = int.Parse(input[1]);
                switch (input[0])
                {
                    case "hp":
                        result.hp = num;
                        break;
                    case "sp":
                        result.sp = num;
                        break;
                    case "attack":
                        result.attack = num;
                        break;
                    case "guard":
                        result.guard = num;
                        break;
                    default:
                        Debug.LogWarning($"PlayerDB-paramGrow:想定されていないパラメーターがあります." +
                            $"data={growdata}");
                        break;
                }
            }
            catch (Exception e) when (e is IndexOutOfRangeException ||
                                     e is FormatException)
            {
                Debug.LogError($"PlayerDB-paramGrow:値に異常があります.:data={growdata}" +
                    $"\n{e}");
            }
            catch (Exception e)
            {
                Debug.LogError($"PlayerDB-paramGrow:関連した異常があります.:data={growdata}" +
                    $"\n{e}");
            }
        }
        return result;
    }
}
