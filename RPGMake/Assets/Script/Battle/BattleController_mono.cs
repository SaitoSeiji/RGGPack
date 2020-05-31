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
        BattleEnd,
        BattleEnd_public//戦闘終了処理も終わったことを示す
    }
    BattleState _battleState;
    
    public CharcterField _charcterField;
    Queue<BattleChar> _battleCharQueue = new Queue<BattleChar>();

    public bool _waitInput { get; private set; } = false;
    
    (ICommandData command, BattleChar target) _charInput=(null,null);
    
    #region callback
    //_battleAction_command～battleAction_endTurnまでを一つにまとめたい
    public Action _battleAction_encount;
    public Action _battleAction_waitInput;
    public Action<BattleChar, SkillCommandData> _battleAction_command;
    public Action<BattleChar, ItemData> _battleAction_item;
    public Action<bool, bool, BattleChar, int> _battleAction_attack;//(isCure,isDefeat,target,actNum)
    public Action _battleAction_endTurn;
    public Action<bool,int,int> _battleAction_end;//islose,money,exp
    #endregion

    public BattleController(SavedDBData_player player,List<SavedDBData_char> enemy)
    {
        _charcterField = new CharcterField(new List<PlayerChar>() {new PlayerChar( player)},enemy.Select(x=>new EnemyChar(x)).ToList());
        CharcterField.SetUniqueName(_charcterField._enemyList);
        _charcterField.SetRival();
        _battleCharQueue = SetFirstQueue(_charcterField);
        _battleState = BattleState.WaitInput;
    }

    public void StartBattle()
    {
        _battleAction_encount?.Invoke();
        PrepareNextTurn();
    }
    #region private
    #region 戦闘ロジック
    static Queue<BattleChar> SetFirstQueue(CharcterField cf)
    {
        var result = new Queue<BattleChar>();
        cf._playerList.ForEach(x => result.Enqueue(x));
        cf._enemyList.ForEach(x => result.Enqueue(x));
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
        if (_waitInput || IsBattleEnd_local()) return;
        //次に動くキャラを決める
        var next = _battleCharQueue.Peek();
        CommandCallBack(_charInput.command, next);
        var strategy = CommandStrategy.GetStrategy(_charInput.command);
        strategy.TurnAction(next, _charInput.target, _charInput.command, _battleAction_attack,
            _charcterField.GetFriend(next),_charcterField.GetEnemy(next));

        _battleAction_endTurn?.Invoke();
        _battleCharQueue.Dequeue();
        _battleCharQueue.Enqueue(next);
    }
    void PrepareNextTurn()
    {
        if (IsBattleEnd_local())
        {
            return;
        }
        //死亡したキャラをはじく
        while (!_battleCharQueue.Peek().IsAlive())
        {
            _battleCharQueue.Dequeue();
        }

        foreach (var enemy in _charcterField._enemyList)
        {
            if (!enemy.IsAlive())
            {
                _charcterField._playerList.ForEach(x => x.RemoveRaival(enemy));
            }
        }

        if (_battleCharQueue.Peek() is PlayerChar)
        {
            _waitInput = true;
            _battleAction_waitInput?.Invoke();
        }
    }
    #endregion

    void CommandCallBack(ICommandData icommand,BattleChar target)
    {
        if(icommand is SkillCommandData)
        {
            _battleAction_command?.Invoke(target,icommand as SkillCommandData);
        }else if(icommand is ItemData)
        {
            _battleAction_item?.Invoke(target, icommand as ItemData);
        }
    }

    static (int money,int exp) CalcMoney_exp(List<EnemyChar> enemyList)
    {
        int money = 0;
        int exp = 0;
        enemyList.ForEach(x =>
        {
            money += x._money;
            exp += x._exp;
        });
        return (money,exp);
    }
    #endregion
    #region public
    bool IsBattleEnd_local()
    {
        var plDead= _charcterField.AllDead(_charcterField._playerList.Select(x => (BattleChar)x).ToList());
        var eneDead=_charcterField.AllDead(_charcterField._enemyList.Select(x => (BattleChar)x).ToList());
        return plDead || eneDead;
    }

    public bool IsBattleEnd_public()
    {
        return _battleState == BattleState.BattleEnd_public;
    }
    public List<BattleChar> GetTargetPool(ICommandData targetIntarface)
    {
        var next = _battleCharQueue.Peek();
        var commandData = targetIntarface.GetCommandData();
        return Battle_targetDicide.GetTargetPool(commandData._target, next, _charcterField.GetFriend(next), _charcterField.GetEnemy(next));
    }

    public bool CommandUseable(string commandName)
    {
        var next =(PlayerChar) _battleCharQueue.Peek();
        var command = next.GetCommand(commandName);
        var bur = new Battle_useResource(command._useResourceType,command._useNum, next);
        return bur.IsUseable();
    }

    public void SetCharInput(string charName,ICommandData command)
    {
        var target = _battleCharQueue.Where(x => x._displayName == charName).FirstOrDefault();
        _charInput = (command, target);
        _waitInput = false;
    }

    public void Battle()
    {
        if(IsBattleEnd_local()) _battleState = BattleState.BattleEnd;
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
                bool losePl = _charcterField.AllDead(_charcterField._playerList.Select(x => (BattleChar)x).ToList());
                if (losePl)
                {
                    EventCodeReadController.Instance.RetireEvent();
                }
                var resultData = CalcMoney_exp(_charcterField._enemyList);
                _battleAction_end?.Invoke(losePl,resultData.money,resultData.exp);
                _battleState = BattleState.BattleEnd_public;
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

    public bool IsBattleEnd()
    {
        return battle.IsBattleEnd_public() && !BattleUIController.Instance.IsBattleNow();
    }
    #endregion
    void StartBattle(SavedDBData_player player,EnemySetData enemys)
    {
        UIController.Instance.AddUI(_battleUI._BaseUI,true);
        battle = new BattleController(player, enemys._charList.Select(x => x._charData).ToList());
        _battleUI.SetUpCharData();
        _battleUI.SetUpBattleDelegate(battle);
        battle._battleAction_end += BattleEndCallBack;
        battle.StartBattle();
    }

    public void StartBattle(EnemySetData enemys)
    {
        var dblist= SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>();
        var _playerSaveData = dblist[0];
        StartBattle(_playerSaveData, enemys);
    }

    void BattleEndCallBack(bool lose,int money,int exp)
    {
        if (!lose)
        {
            var pl = battle._charcterField._playerList;
            pl.ForEach(x => {
                x._PlayerData._exp += exp;
                SaveDataController.Instance.SetData<PlayerDB, SavedDBData_player>(x._PlayerData);
            });

            var party = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
            party._haveMoney += money;
            SaveDataController.Instance.SetData<PartyDB, SavedDBData_party>(party);
        }
    }
}
