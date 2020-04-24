using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleController
{
    public PlayerChar _player { get; private set; }
    public List<EnemyChar> _enemys { get; private set; }
    Queue<BattleChar> _battleCharQueue = new Queue<BattleChar>();

    public bool _waitInput { get; private set; } = false;

    //(int index, int charIndex) _charInput=(-1,-1);
    (string command, string charName) _charInput=("","");
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
        if (IsEnd())
        {
            AddLog_End();
            return;
        }
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
        AddLog_test_hpResult();
        if(_waitInput) AddLog_test_playerTurn();
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
            target = next.SelectTarget(_charInput.charName);
            command = next.SelectCommand(_charInput.command);
            damage = next.SelectAttack(command._skillName);
            _charInput = ("","");
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

    public void SetCharInput(string charName,string command)
    {
        _charInput = (command, charName);
        _waitInput = false;
    }
    #endregion
    #region log
    void AddLog_test_hpResult()
    {
        //_logText += string.Format("player hp{0}\n", _player._myCharData._hp);
        //foreach (var enemy in _enemys)
        //{
        //    _logText += string.Format("{0} hp {1}\n", enemy._myCharData._name, enemy._myCharData._hp);
        //}
    }
    void AddLog_test_playerTurn()
    {
        //_logText += "next is playerTurn\n";
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

    void AddLog_End()
    {
        if (_player._nowHp <= 0)
        {
            _logText += string.Format("$目の前が真っ暗になった\n");
        }
        else
        {
            _logText += string.Format("$コウは戦闘に勝利した！\n");
            _logText += string.Format("経験値やお金を手に入れた！\n");
        }
    }
    public string GetLog()
    {
        string result = (string)_logText.Clone();
        _logText = "";
        return result;
    }
    #endregion
}

public class BattleController_mono : SingletonMonoBehaviour<BattleController_mono>
{
    [SerializeField] BattleUIController _battleUI;
    [SerializeField] BattleCharScriptable _player;
    [SerializeField] EnemySetScriptable _enemys;
    WaitFlag wf = new WaitFlag();
    public BattleController battle { get; private set; }

    //[SerializeField] int selectIndex;
    //[SerializeField] int seletCommand;
    
    public void SetChar(BattleCharScriptable pl, EnemySetScriptable ene)
    {
        pl = _player;
        _enemys = ene;
        battle = new BattleController(_player._CharData
            ,ene._enemySetData._charList.Select(x=>x._CharData).ToList());
    }

    public void SetCharInput(string target,string skill)
    {
        battle.SetCharInput(target, skill);
    }

    public void Next()
    {
        if (battle._waitInput)
        {
            return;
        }
        else
        {
            battle.Command();
            BattleUIController.Instance.AddDisplayText(battle.GetLog());
        }
    }


    public List<EnemyChar> GetEnemyList()
    {
        return battle._enemys;
    }

    public List<SkillDBData> GetSkillList()
    {
        return battle._player._myCharData._mySkillList;
    }

    public bool IsEnd()
    {
        return battle.IsEnd();
    }
    
    public void StartBattle(BattleCharScriptable player,EnemySetScriptable enemys)
    {
        UIController.Instance.AddUI(_battleUI._SelfUI);
        SetChar(player, enemys);
        _battleUI.StartBattle();
    }

    [ContextMenu("testBattle")]
    void SetCharText()
    {
        StartBattle(_player, _enemys);
    }
}
