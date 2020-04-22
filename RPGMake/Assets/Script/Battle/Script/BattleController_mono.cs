using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleController
{
    PlayerChar _player;
    List<EnemyChar> _enemys;
    Queue<BattleChar> _battleCharQueue = new Queue<BattleChar>();

    public bool _waitInput { get; private set; } = false;

    (int index, int charIndex) _charInput=(-1,-1);
    string _logText = "";

    public BattleController(BattleCharData player,List<BattleCharData> enemy)
    {
        _player = new PlayerChar(player);
        _enemys = enemy.Select(x=>new EnemyChar(x)).ToList();

        foreach(var ene in _enemys)
        {
            ene.AddRaival(_player);
            _player.AddRaival(ene);
        }
        _battleCharQueue = SetFirstQueue();
        PrepareNextTurn();
    }
    #region private
    Queue<BattleChar> SetFirstQueue()
    {
        var result = new Queue<BattleChar>();
        result.Enqueue(_player);
        foreach(var enemy in _enemys)
        {
            result.Enqueue(enemy);
        }
        return result;
    }

    void PrepareNextTurn()
    {
        SyncDeadChar();
        if (_battleCharQueue.Peek() is PlayerChar)
        {
            _waitInput = true;
            
        }

        foreach (var enemy in _enemys)
        {
            if (!enemy.IsAlive())
            {
                _player.RemoveRaival(enemy);
            }
        }
        AddLog_hpResult();
        if(_waitInput) AddLog_playerTurn();
    }
    void SyncDeadChar()
    {
        var tempQueue=new Queue<BattleChar>();
        while (_battleCharQueue.Count > 0)
        {
            var target = _battleCharQueue.Dequeue();
            if (!target.IsAlive())
            {
                AddLog_defeat(target._myCharData);
            }
            else
            {
                tempQueue.Enqueue(target);
            }

        }
        _battleCharQueue = tempQueue;
    }

    private bool CheckIsAliveTarget(int index)
    {
        if (_enemys.Count <= index) return false;
        return _enemys[index].IsAlive();
    }
    #endregion
    #region public
    public bool IsEnd()
    {
        if (!_player.IsAlive()) return true;
        foreach(var enemy in _enemys)
        {
            if (enemy.IsAlive()) return false;
        }
        return true;
    }
    
    public void Command()
    {
        if (_waitInput||IsEnd()) return;
        var next = _battleCharQueue.Dequeue();
        BattleChar target;
        SkillCommandData command;
        int damage=0;
        if(next is PlayerChar)
        {
            target= next.SelectTarget(_charInput.index);
            command = next.SelectCommand(_charInput.charIndex);
            damage = next.SelectAttack(command._skillName);
            _charInput = (-1, -1);
        }
        else
        {
            target = next.SelectTargetAuto();
            command = next.SelectCommand_auto();
            damage = next.SelectAttack(command._skillName);
        }
        var damaged= target.SetDamage(damage);
        _battleCharQueue.Enqueue(next);
        AddLog_command(next._myCharData, command);
        AddLog_damage(target._myCharData, damaged);
        PrepareNextTurn();
    }
    public void SetCharInput(int index,int command)
    {
        if (!CheckIsAliveTarget(index)) return;
        _charInput = (index, command);
        _waitInput = false;
    }
    #endregion
    void AddLog_hpResult()
    {
        _logText += string.Format("player hp{0}\n", _player._myCharData._hp);
        foreach (var enemy in _enemys)
        {
            _logText += string.Format("{0} hp {1}\n", enemy._myCharData._name, enemy._myCharData._hp);
        }
    }
    void AddLog_playerTurn()
    {
        _logText += "next is playerTurn\n";
    }
    void AddLog_command(BattleCharData chars,SkillCommandData skilldata)
    {
        _logText += string.Format("{0}の{1}\n",chars._name,skilldata._skillName);
    }
    void AddLog_damage(BattleCharData chars, int damage)
    {
        _logText += string.Format("{0}は{1}のダメージを受けた\n", chars._name, damage);
    }
    void AddLog_defeat(BattleCharData chars)
    {
        _logText += string.Format("{0}は倒れた\n",chars._name);
    }
    public string GetLog()
    {
        string result = (string)_logText.Clone();
        _logText = "";
        return result;
    }
}

public class BattleController_mono : MonoBehaviour
{
    [SerializeField] BattleCharScriptable player;
    [SerializeField] List<BattleCharScriptable> enemys;
    WaitFlag wf = new WaitFlag();
    BattleController battle;

    [SerializeField] int selectIndex;
    [SerializeField] int seletCommand;

    private void Start()
    {
        battle = new BattleController(player._CharData,enemys.Select(x=>x._CharData).ToList());
        wf.SetWaitLength(1f);
    }

    private void Update()
    {
        if (!battle.IsEnd()&&!wf._waitNow)
        {
            if (battle._waitInput)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    battle.SetCharInput(selectIndex, seletCommand);
                }
            }
            else
            {
                wf.WaitStart();
                battle.Command();
                Debug.Log(battle.GetLog());
            }
        }
    }
}
