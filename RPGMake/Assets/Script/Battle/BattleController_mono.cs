using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleChar
{
    [SerializeField] int _hp;
    public int _Hp { get { return _hp; } }
    [SerializeField]int _attack;
    [SerializeField] int _guard;
    BattleChar _enemy;

    public BattleChar(int hp, int attack,int guard)
    {
        _hp = hp;
        _attack = attack;
    }

    public void SetEnemy(BattleChar enemy)
    {
        _enemy = enemy;
    }

    public int GetAttack()
    {
        return _attack;
    }

    public void SetDamage(int damage)
    {
        _hp -=CalcDamage(damage);
        if (_hp < 0) _hp = 0;
    }

    public bool IsAlive()
    {
        return _Hp > 0;
    }


    public BattleChar GetTarget()
    {
        return _enemy;
    }

    int CalcDamage(int attack)
    {
        var result= attack - _guard;
        if (result <= 0) result = 1;
        return result;
    }
}

public class BattleController
{
    BattleChar _charA;
    BattleChar _charB;
    BattleChar _nextTurnChar;

    public BattleController(BattleChar a, BattleChar b)
    {
        _charA = a;
        _charB = b;
        _charA.SetEnemy(_charB);
        _charB.SetEnemy(_charA);
    }

    public bool IsEnd()
    {
        return !_charA.IsAlive() || !_charB.IsAlive();
    }
    
    public void Command()
    {
        var attack= _nextTurnChar.GetAttack();
        var target = _nextTurnChar.GetTarget();
        target.SetDamage(attack);
    }

    public void SetNextTurn()
    {
        _nextTurnChar = GetNextTurnChar();
    }

    BattleChar GetNextTurnChar()
    {
        if (_nextTurnChar == null)
        {
            return _charA;
        }
        else if(_nextTurnChar==_charA)
        {
            return _charB;
        }else if (_nextTurnChar == _charB)
        {
            return _charA;
        }
        return null;
    }


    public string GetBattleLog()
    {
        string result = "";
        result+= string.Format("A hp{0}\n B hp{1}\n",_charA._Hp,_charB._Hp);
        return result;
    }

}

public class BattleController_mono : MonoBehaviour
{
    [SerializeField] BattleChar a;
    [SerializeField] BattleChar b;
    WaitFlag wf = new WaitFlag();
    BattleController battle;
    private void Start()
    {
        battle = new BattleController(a,b);
        wf.SetWaitLength(0.1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)&&!battle.IsEnd())
        {
            wf.WaitStart();
            battle.SetNextTurn();
            battle.Command();
            Debug.Log(battle.GetBattleLog());

        }
    }
}
