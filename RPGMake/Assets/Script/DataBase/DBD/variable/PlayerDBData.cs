using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

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

    [Serializable]
    public class LevelSkillData
    {
        public int _level;
        public SkillDBData _targetSkill;
        public LevelSkillData(int level,SkillDBData targetSkill)
        {
            _level = level;
            _targetSkill = targetSkill;
        }

        public bool EqualLevel(LevelSkillData check)
        {
            return _level == check._level;
        }
    }
    [SerializeField] public List<LevelSkillData> _levelSkillData = new List<LevelSkillData>();

    #endregion

    #region データの操作
    public void InitNeedExpList()
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
        UpdateSkill(_level-up,_level);
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
    
    void UpdateSkill(int fromlevel,int nowlevel)
    {
        var addlist=GetBetweenSkill(fromlevel, nowlevel);
        addlist.ForEach(x => AddSkill(x));
    }
    #endregion
    #region 加工後データの取得
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

    public int CalcNextLevelExp()
    {
        return GetTargetLevelExp_sum(_level) - _exp;
    }


    public List<SkillDBData> GetBetweenSkill(int fromlevel, int nowlevel)
    {
        var result = _levelSkillData.Where(x => fromlevel < x._level && x._level < nowlevel).Select(x => x._targetSkill).ToList();
        return result;
    }
    #endregion
}

[CreateAssetMenu(fileName = "PlayerDBData", menuName = "DataBases/Data/PlayerDBData", order = 0)]
public class PlayerDBData : VariableDBData
{
    [SerializeField] SavedDBData_player _charData = new SavedDBData_player();
    [SerializeField, NonEditable] List<string> _skillNameSet = new List<string>();
    [SerializeField, NonEditable] List<string> _levelSkillNameSet = new List<string>();


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
        _charData._paramGrowData = UpdateMember_growData(data.GetData_list("paramGrow"));
        _charData.InitNeedExpList();
        _levelSkillNameSet = data.GetData_list("levelSkill");
    }

    public override void RateUpdateMemeber()
    {
        base.RateUpdateMemeber();
        var temp = (SavedDBData_char)_charData;
        Partial_CharcterDBData.RateUpdateMemeber(ref temp, _skillNameSet,this);
        List<string> addedSkillName = _skillNameSet;//重複スキルのチェック
        //レベルとスキルの対応データの追加
        _charData._levelSkillData = new List<SavedDBData_player.LevelSkillData>();
        var skillDB = SaveDataController.Instance.GetDB_static<SkillDB>()._dataList;
        foreach (var data in _levelSkillNameSet)
        {
            try
            {
                var input = data.Split(' ');
                var levelData = int.Parse(input[0]);
                var skillData = skillDB.Where(x => x._serchId == input[1]).First();
                var add = new SavedDBData_player.LevelSkillData(levelData, skillData);

                if (!addedSkillName.Contains(input[1]))
                {
                    _charData._levelSkillData.Add(add);
                    addedSkillName.Add(input[1]);
                }
                else
                {
                    ThrowErrorLog(null, data, "スキルの重複があります");
                }
            }
            catch (Exception e) when (e is FormatException ||
                                      e is IndexOutOfRangeException)
            {
                ThrowErrorLog(e, data, ErrorCode_format);
            }
            catch (InvalidOperationException e)
            {
                ThrowErrorLog(e, data, ErrorCode_uncollectName);
            }
        }
        //同名スキルを覚えた時のエラー表示が欲しい


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
