using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleChar
{
    public SavedDBData_char _myCharData { get; protected set; }
    public int _maxHp { get; protected set; }
    public int _nowHp { get; protected set; }
    protected List<BattleChar> _enemyTargets = new List<BattleChar>();

    public BattleChar(SavedDBData_char charData)
    {
        _myCharData = charData;
        _nowHp = _myCharData._HpMax;
        _maxHp = _myCharData._HpMax;
    }

    public void AddRaival(BattleChar enemy)
    {
        _enemyTargets.Add(enemy);
    }

    public void RemoveRaival(BattleChar enemy)
    {
        if (_enemyTargets.Contains(enemy)) return;
        _enemyTargets.Remove(enemy);
    }
    #region selctTarget

    public BattleChar SelectTargetAuto()
    {
        return SelectTargetAI();
    }

    protected virtual BattleChar SelectTargetAI()
    {
        BattleChar result = null;
        var targetIndex = UnityEngine.Random.Range(0, _enemyTargets.Count);
        result = _enemyTargets[targetIndex];
        return result;
    }
    #endregion
    #region attack
    public int CalcurateAttack(float rate)
    {
        return (int)(_myCharData._attack * rate);
    }

    public SkillCommandData SelectCommand(int index)
    {
        return _myCharData._mySkillList[index]._Data;
    }
    public SkillCommandData GetCommand(string name)
    {
        if (string.IsNullOrEmpty(name)) return SelectCommand_auto();
        var list = SaveDataController.Instance.GetDB_static<SkillDB>()._dataList;
        var command = list.Where(x => x._Data._skillName == name).First();
        return command._Data;
    }

    public SkillCommandData SelectCommand_auto()
    {
        int select= Random.Range(0, _myCharData._mySkillList.Count);
        return _myCharData._mySkillList[select]._Data;
    }
    #endregion
    #region damage
    /// <summary>
    /// calcダメージしてから使う
    /// </summary>
    /// <param name="damage"></param>
    public virtual void SetDamage(int damage)
    {
        _nowHp -= damage;
        if (_nowHp < 0) _nowHp = 0;
    }
    public int CalcDamage(int attack)
    {
        var result = attack - _myCharData._guard;
        if (result <= 0) result = 1;
        return result;
    }

    public void SetCure(int cure)
    {
        _nowHp += cure;
        if (_maxHp < _nowHp) _nowHp = _maxHp;
        
    }
    #endregion
    public bool IsAlive()
    {
        return _nowHp > 0;
    }
}
public class PlayerChar : BattleChar
{
    SavedDBData_player _charData;
    public int _maxSP { get; private set; }
    public int _nowSP { get; private set; }
    public new SavedDBData_player _myCharData
    {
        get
        {
            SyncData();
            return _charData;
        }
    }
    public PlayerChar(SavedDBData_player charData) : base(charData)
    {
        _charData = charData;
        _nowHp = _charData._hpNow;
        _maxSP = charData._spMax;
        _nowSP = charData._spNow;
    }

    //_charDataに反映
    void SyncData()
    {
        _charData = _charData.Copy(base._myCharData);
        _charData._hpNow = _nowHp;
        _charData._spNow = _nowSP;
    }

    public void UseSP(int use)
    {
        _nowSP -= use;
        _nowSP = Mathf.Clamp(_nowSP,0,_maxSP);
        SyncData();
    }

    public void CureSP(int cure)
    {
        _nowSP += cure;
        _nowSP = Mathf.Clamp(_nowSP, 0, _maxSP);
        SyncData();
    }
    
}
public class EnemyChar : BattleChar
{
    public EnemyChar(SavedDBData_char charData) : base(charData)
    {

    }
}
