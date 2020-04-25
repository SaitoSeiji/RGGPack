using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleChar
{
    public BattleCharData _myCharData { get; protected set; }
    public int _nowHp { get; private set; }
    protected List<BattleChar> _enemyTargets = new List<BattleChar>();

    public BattleChar(BattleCharData charData)
    {
        _myCharData = charData;
        _nowHp = _myCharData._Hp;
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
        var command = _myCharData._mySkillList.Where(x => x._SKill._skillName == name).FirstOrDefault();
        if (command == null) command = _myCharData._mySkillList[0];
        return command._SKill;
    }

    public SkillCommandData SelectCommand_auto()
    {
        int select= Random.Range(0, _myCharData._mySkillList.Count);
        return _myCharData._mySkillList[select]._SKill;
    }
    #endregion
    #region damage
    public int SetDamage(int damage)
    {
        _nowHp -= CalcDamage(damage);
        if (_nowHp < 0) _nowHp = 0;
        return CalcDamage(damage);
    }
    int CalcDamage(int attack)
    {
        var result = attack - _myCharData._guard;
        if (result <= 0) result = 1;
        return result;
    }
    #endregion
    public bool IsAlive()
    {
        return _nowHp > 0;
    }
}
public class PlayerChar : BattleChar
{
    public PlayerChar(BattleCharData charData) : base(charData)
    {

    }
}
public class EnemyChar : BattleChar
{
    public EnemyChar(BattleCharData charData) : base(charData)
    {

    }
}
