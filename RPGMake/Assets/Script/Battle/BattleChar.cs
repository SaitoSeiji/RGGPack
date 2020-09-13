using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleChar
{
    protected SavedDBData_char _myCharData { get; protected private set; }
    public string _displayName;
    public int _maxHp { get; protected set; }
    public int _nowHp { get; protected set; }
    protected List<BattleChar> _enemyTargets = new List<BattleChar>();

    #region _myCharDataへのアクセス
    public int _money { get { return _myCharData._money; } }
    public int _exp { get { return _myCharData._exp; } }
    public Sprite _image { get { return _myCharData._charImage; } }
    #endregion

    public BattleChar(SavedDBData_char charData)
    {
        _myCharData = charData;
        _nowHp = _myCharData._HpMax;
        _maxHp = _myCharData._HpMax;
        _displayName = _myCharData._Name;
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

    public BattleChar SelectTargetAuto(List<BattleChar> targetPool)
    {
        return SelectTargetAI(targetPool);
    }

    protected virtual BattleChar SelectTargetAI(List<BattleChar> targetPool)
    {
        BattleChar result = null;
        var targetIndex = UnityEngine.Random.Range(0, targetPool.Count);
        result = targetPool[targetIndex];
        return result;
    }
    #endregion
    #region attack
    public int CalcurateAttack(float rate)
    {
        return (int)(_myCharData._Attack_withAdd * rate);
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
        var result = attack - _myCharData._Guard_withAdd;
        if (result <= 0) result = 1;
        return result;
    }

    public virtual void SetCure(int cure)
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
    public int _maxSP { get; private set; }
    public int _nowSP { get; private set; }

    public SavedDBData_player _PlayerData
    {
        get;private set;
    }

    public PlayerChar(SavedDBData_player charData) : base(charData)
    {
        _PlayerData = charData;
        _nowHp = charData._hpNow;
        _maxSP = charData._spMax;
        _nowSP = charData._spNow;
    }

    //_charDataに反映
    public void SyncData_This2Data()
    {
        _PlayerData._hpNow = _nowHp;
        _PlayerData._spNow = _nowSP;
    }
    public void SyncData_Data2This()
    {
        _maxHp = _PlayerData._hpMax;
        _nowHp = _PlayerData._hpNow;
        _maxSP = _PlayerData._spMax;
        _nowSP = _PlayerData._spNow;
    }

    public void UseSP(int use)
    {
        _nowSP -= use;
        _nowSP = Mathf.Clamp(_nowSP,0,_maxSP);
    }

    public void CureSP(int cure)
    {
        _nowSP += cure;
        _nowSP = Mathf.Clamp(_nowSP, 0, _maxSP);
    }
    
}
public class EnemyChar : BattleChar
{
    public EnemyChar(SavedDBData_char charData) : base(charData)
    {

    }
}
