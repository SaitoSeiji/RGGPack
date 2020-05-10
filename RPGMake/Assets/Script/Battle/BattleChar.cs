using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleChar
{
    public BattleCharData _myCharData { get; protected set; }
    public int _maxHp { get; protected set; }
    public int _nowHp { get; protected set; }
    protected List<BattleChar> _enemyTargets = new List<BattleChar>();

    public BattleChar(BattleCharData charData)
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
    public BattleChar SelectTarget(int i)
    {
        return _enemyTargets[i];
    }
    public BattleChar SelectTarget(string st)
    {
        return _enemyTargets.Where(x => x._myCharData._name == st).FirstOrDefault();
    }

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
    public int SelectAttack(string name)
    {
        float rate = -1;
        var targetSkill = _myCharData._mySkillList.Where(x => x._SKill._skillName == name).FirstOrDefault();
        if (targetSkill != null)
        {
            rate = targetSkill._SKill.GetRate();
        }
        else
        {
            rate = 1.0f;
        }
        return (int)(_myCharData._attack * rate);
    }

    public SkillCommandData SelectCommand(int index)
    {
        return _myCharData._mySkillList[index]._SKill;
    }
    public SkillCommandData SelectCommand(string name)
    {
        if (string.IsNullOrEmpty(name)) return SelectCommand_auto();
        var list = SaveDataController.Instance.GetDB_static<SkillDB>()._dataList;
        var command = list.Where(x => x._SKill._skillName == name).First();
        return command._SKill;
    }

    public SkillCommandData SelectCommand_auto()
    {
        int select= Random.Range(0, _myCharData._mySkillList.Count);
        return _myCharData._mySkillList[select]._SKill;
    }
    #endregion
    #region damage
    /// <summary>
    /// calcダメージしてから使う
    /// </summary>
    /// <param name="damage"></param>
    public void SetDamage(int damage)
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
    PlayerCharData _charData;
    public new PlayerCharData _myCharData { get
        {
            SyncData();
            return _charData;
        }
    }
    public PlayerChar(PlayerCharData charData) : base(charData)
    {
        _charData = charData;
        _nowHp = _charData._hpNow;
    }

    void SyncData()
    {
        _charData = _charData.Copy(base._myCharData);
        _charData._hpNow = _nowHp; ;
    }
}
public class EnemyChar : BattleChar
{
    public EnemyChar(BattleCharData charData) : base(charData)
    {

    }
}
