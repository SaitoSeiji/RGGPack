using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleController
{
    public PlayerChar _player { get; private set; }
    public List<EnemyChar> _enemys { get; private set; } = new List<EnemyChar>();
    Queue<BattleChar> _battleCharQueue = new Queue<BattleChar>();

    public bool _waitInput { get; private set; } = false;

    //(int index, int charIndex) _charInput=(-1,-1);
    (string command, string charName) _charInput=("","");
    //string _logText = "";
    //_battleAction_command～battleAction_endTurnまでを一つにまとめたい
    public Action _battleAction_encount;
    public Action<BattleCharData,SkillCommandData> _battleAction_command;
    public Action<BattleCharData,int> _battleAction_damage;
    public Action<BattleCharData> _battleAction_defeat;
    public Action _battleAction_endTurn;
    public Action<bool> _battleAction_end;
    

    public BattleController(BattleCharData player,List<BattleCharData> enemy)
    {
        _player = new PlayerChar(player);
        _enemys = new List<EnemyChar>();
        _enemys = enemy.Select(x=>new EnemyChar(x)).ToList();
        SetUniqueName(_enemys);
        foreach(var ene in _enemys)
        {
            ene.AddRaival(_player);
            _player.AddRaival(ene);
        }
        _battleCharQueue = SetFirstQueue();
    }

    public void StartBattle()
    {
        PrepareNextTurn();
        _battleAction_encount?.Invoke();
    }
    #region private
    //複数同名モンスターがいるときに固有名にする AとかBとか
    //無駄に長いので短くできそう
    void SetUniqueName(List<EnemyChar> enemys)
    {
        var temp = new List<EnemyChar>(enemys);
        while (temp.Count > 0)
        {
            string targetName = temp[0]._myCharData._name;
            var targets = temp.Where(x => x._myCharData._name == targetName).ToArray();
            if (targets.Length > 1)
            {
                for(int i = 0; i < targets.Length; i++)
                {
                    targets[i]._myCharData._name += ConvertNum(i);
                }
            }
            foreach(var t in targets)
            {
                temp.Remove(t);
            }
        }
    }

    static string ConvertNum(int num)
    {
        switch (num)
        {
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
            case 4:
                return "E";
        }
        return "";
    }

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
        if (IsEnd())
        {
            _battleAction_end?.Invoke(_player._nowHp <= 0);
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
    }
    BattleCharData SyncDeadChar()
    {
        BattleCharData deadman = null;
        var tempQueue=new Queue<BattleChar>();
        while (_battleCharQueue.Count > 0)
        {
            var target = _battleCharQueue.Dequeue();
            if (!target.IsAlive())
            {
                //AddLog_defeat(target._myCharData);
                deadman = target._myCharData;
            }
            else
            {
                tempQueue.Enqueue(target);
            }
        }
        _battleCharQueue = tempQueue;
        return deadman;
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
        if(next is PlayerChar)
        {
            target = next.SelectTarget(_charInput.charName);
            command = next.SelectCommand(_charInput.command);
            _charInput = ("","");
        }
        else
        {
            target = next.SelectTargetAuto();
            command = next.SelectCommand_auto();
        }
        int damage = next.SelectAttack(command._skillName);
        target.SetDamage(damage);
        _battleCharQueue.Enqueue(next);
        var deadMan= SyncDeadChar();
        //var log = GetLog_command(next._myCharData,command) + GetLog_damage(target._myCharData,damage) + GetLog_defeat(deadMan);
        _battleAction_command?.Invoke(next._myCharData,command);
        _battleAction_damage?.Invoke(target._myCharData,damage);
        _battleAction_defeat?.Invoke(deadMan);
        _battleAction_endTurn?.Invoke();
        PrepareNextTurn();
    }

    public void SetCharInput(string charName,string command)
    {
        _charInput = (command, charName);
        _waitInput = false;
    }
    #endregion
    #region log
    
    #endregion
}

public class BattleController_mono : SingletonMonoBehaviour<BattleController_mono>
{
    [SerializeField] BattleUIController _battleUI;
    [SerializeField] CharcterDBData _player;
    [SerializeField] EnemySetDBData _enemys;
    WaitFlag wf = new WaitFlag();
    public BattleController battle { get; private set; }
    
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
        }
    }
    #region get

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

    public bool IsBattleEnd()
    {
        return IsEnd() && !BattleUIController.Instance.IsBattleNow();
    }
    #endregion
    public void StartBattle(BattleCharData player,EnemySetData enemys)
    {
        UIController.Instance.AddUI(_battleUI._BaseUI,true);
        battle = new BattleController(player, enemys._charList.Select(x => x._CharData).ToList());
        _battleUI.SetUpCharData();
        _battleUI.SetUpBattleDelegate(battle);
        battle.StartBattle();
    }

    public void StartBattle(EnemySetData enemys)
    {
        StartBattle(_player._CharData, enemys);
    }



    [ContextMenu("testBattle")]
    void SetCharText()
    {
        StartBattle(_player._CharData, _enemys._enemySetData);
    }
}
