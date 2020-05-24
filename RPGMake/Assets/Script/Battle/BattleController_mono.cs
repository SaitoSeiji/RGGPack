using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DBDInterface;

public class BattleController
{
    public enum BattleState
    {
        WaitInput,
        TurnAction,
        PrepareNextTurn,
        BattleEnd
    }
    BattleState _battleState;

    public PlayerChar _player { get; private set; }
    public List<EnemyChar> _enemys { get; private set; } = new List<EnemyChar>();
    Queue<BattleChar> _battleCharQueue = new Queue<BattleChar>();

    public bool _waitInput { get; private set; } = false;
    
    (ICommandData command, BattleChar target) _charInput=(null,null);
    
    #region callback
    //_battleAction_command～battleAction_endTurnまでを一つにまとめたい
    public Action _battleAction_encount;
    public Action _battleAction_waitInput;
    public Action<SavedDBData_char, SkillCommandData> _battleAction_command;
    public Action<SavedDBData_char, ItemData> _battleAction_item;
    public Action<bool, bool, SavedDBData_char, int> _battleAction_attack;//(isCure,isDefeat,target,actNum)
    public Action _battleAction_endTurn;
    public Action<bool> _battleAction_end;
    #endregion

    public BattleController(SavedDBData_player player,List<SavedDBData_char> enemy)
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
        _battleState = BattleState.WaitInput;
    }

    public void StartBattle()
    {
        _battleAction_encount?.Invoke();
        PrepareNextTurn();
    }
    #region private
    #region name
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
    #endregion
    #region 戦闘ロジック
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
    
    //入力を受け取る
    void GetInput()
    {
        //敵の場合はここで入力を作成
        if (!(_battleCharQueue.Peek() is PlayerChar))
        {
            var next = _battleCharQueue.Peek();
            var command = next.SelectCommand_auto() as ICommandData;
            var target = next.SelectTargetAuto();
            _charInput = (command, target);
        }
    }

    //戦闘ロジックの実行
    void TurnAct()
    {
        if (_waitInput || IsEnd()) return;
        //次に動くキャラを決める
        var next = _battleCharQueue.Peek();
        CommandCallBack(_charInput.command, next);
        var strategy = CommandStrategy.GetStrategy(_charInput.command);
        strategy.TurnAction(next, _charInput.target, _charInput.command, _battleAction_attack,
            GetFriend(next),GetEnemy(next));

        _battleAction_endTurn?.Invoke();
        _battleCharQueue.Dequeue();
        _battleCharQueue.Enqueue(next);
    }
    void PrepareNextTurn()
    {
        if (IsEnd())
        {
            return;
        }
        //死亡したキャラをはじく
        while (!_battleCharQueue.Peek().IsAlive())
        {
            _battleCharQueue.Dequeue();
        }

        foreach (var enemy in _enemys)
        {
            if (!enemy.IsAlive())
            {
                _player.RemoveRaival(enemy);
            }
        }

        if (_battleCharQueue.Peek() is PlayerChar)
        {
            _waitInput = true;
            _battleAction_waitInput?.Invoke();
        }
    }
    #endregion
    #region 対象選択用
    List<BattleChar> GetFriend(BattleChar target)
    {
        if (target == _player) return new List<BattleChar>() { _player };
        else return _enemys.Select(x => (BattleChar)x).ToList();
    }
    List<BattleChar> GetEnemy(BattleChar target)
    {
        if (target == _player) return _enemys.Select(x => (BattleChar)x).ToList();
        else return new List<BattleChar>() { _player };
    }

    BattleChar GetNext()
    {
        return _battleCharQueue.Peek();
    }
    //スキルの対象を決める
    //BattleChar GetInputCommandTarget(BattleChar user, Battle_targetDicide ct)
    //{
    //    if (!ct.IsInputSelect()) return null;
    //    if (_charInput.target != null) return _charInput.target;
    //    else return user.SelectTargetAuto();
    //}
    #endregion

    void CommandCallBack(ICommandData icommand,BattleChar target)
    {
        if(icommand is SkillCommandData)
        {
            _battleAction_command?.Invoke(target._myCharData,icommand as SkillCommandData);
        }else if(icommand is ItemData)
        {
            _battleAction_item?.Invoke(target._myCharData, icommand as ItemData);
        }
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
    public List<BattleChar> GetTargetPool(ICommandData targetIntarface)
    {
        var next = _battleCharQueue.Peek();
        var commandData = targetIntarface.GetCommandData();
        return Battle_targetDicide.GetTargetPool(commandData._target, next, GetFriend(next), GetEnemy(next));
    }

    public bool CommandUseable(string commandName)
    {
        var next = _player;
        var command = next.GetCommand(commandName);
        var bur = new Battle_useResource(command._useResourceType,command._useNum, next);
        return bur.IsUseable();
    }

    public void SetCharInput(string charName,ICommandData command)
    {
        var target = _battleCharQueue.Where(x => x._myCharData._name == charName).FirstOrDefault();
        _charInput = (command, target);
        _waitInput = false;
    }

    public void Battle()
    {
        if(IsEnd()) _battleState = BattleState.BattleEnd;
        switch (_battleState)
        {
            case BattleState.WaitInput:
                GetInput();
                _battleState = BattleState.TurnAction;
                break;
            case BattleState.TurnAction:
                TurnAct();
                _battleState = BattleState.PrepareNextTurn;
                break;
            case BattleState.PrepareNextTurn:
                PrepareNextTurn();
                _battleState = BattleState.WaitInput;
                break;
            case BattleState.BattleEnd:
                _battleAction_end?.Invoke(_player._nowHp <= 0);
                break;
        }
    }
    #endregion
}

public class BattleController_mono : SingletonMonoBehaviour<BattleController_mono>
{
    [SerializeField] BattleUIController _battleUI;
    [SerializeField] EnemySetDBData _testButtleEnemys;
    WaitFlag wf = new WaitFlag();

    public BattleController battle { get; private set; }
    //PlayerCharData _playerCharData;

    public void SetCharInput(string target,ICommandData skill)
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
            battle.Battle();
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
    void StartBattle(SavedDBData_player player,EnemySetData enemys)
    {
        UIController.Instance.AddUI(_battleUI._BaseUI,true);
        battle = new BattleController(player, enemys._charList.Select(x => x._CharData).ToList());
        _battleUI.SetUpCharData();
        _battleUI.SetUpBattleDelegate(battle);
        battle._battleAction_end += EndAction;
        battle.StartBattle();
    }

    public void StartBattle(EnemySetData enemys)
    {
        var dblist= SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>();
        var _playerSaveData = dblist[0];
        StartBattle(_playerSaveData, enemys);
    }

    void EndAction(bool lose)
    {
        if (!lose)
        {
            SaveDataController.Instance.SetData<PlayerDB, SavedDBData_player>(battle._player._myCharData);
        }
    }
}
