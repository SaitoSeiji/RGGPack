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
    
    (string command, BattleChar target) _charInput=("",null);

    #region callback
    //_battleAction_command～battleAction_endTurnまでを一つにまとめたい
    public Action _battleAction_encount;
    public Action _battleAction_waitInput;
    public Action<SavedDBData_char, SkillCommandData> _battleAction_command;
    public Action<SavedDBData_char,int> _battleAction_damage;
    public Action<SavedDBData_char,int> _battleAction_cure;
    public Action<SavedDBData_char> _battleAction_defeat;
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
        _charInput = (null, null);
        if (IsEnd())
        {
            _battleAction_end?.Invoke(_player._nowHp <= 0);
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
    //スキルの対象を決める
    BattleChar GetInputCommandTarget(BattleChar user, Battle_targetDicide ct)
    {
        if (!ct.IsInputSelect()) return null;
        if (_charInput.target != null) return _charInput.target;
        else return user.SelectTargetAuto();
    }
    #endregion
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

    public Battle_targetDicide GetCommantTarget(string commandName)
    {
        var next = _battleCharQueue.Peek();
        var command = next.GetCommand(commandName);
        return new Battle_targetDicide(command._target, next, GetFriend(next), GetEnemy(next));
    }

    public bool CommandUseable(string commandName)
    {
        var next = _player;
        var command = next.GetCommand(commandName);
        var bur = new Battle_useResource(command._useResourceType,command._useNum, next);
        return bur.IsUseable();
    }


    public void TurnAct()
    {
        if (_waitInput||IsEnd()) return;
        //次に動くキャラを決める
        var next = _battleCharQueue.Dequeue();
        //使用するコマンドを決める
        SkillCommandData command= next.GetCommand(_charInput.command);
        int attack = next.SelectAttack(command._skillName);
        _battleAction_command?.Invoke(next._myCharData, command);
        bool isPlayer = next is PlayerChar;
        Battle_useResource bur = null;
        //スキルリソース使用処理
        if (isPlayer) {
            var pl = (PlayerChar)next;
            bur = new Battle_useResource(command._useResourceType, command._useNum, pl);
            if (!bur.IsUseable()) return;
            bur.Use();
        }
        //対象を決める
        var btd= new Battle_targetDicide(command._target,next,GetFriend(next), GetEnemy(next));
        var targetPool = btd.GetTargetPool();
        var inputTarget = GetInputCommandTarget(next,btd);
        List<BattleChar> targets=btd.SelectTarget(targetPool,inputTarget);
        //スキルの効果を発動する
        foreach (var target in targets)
        {
            var btr =new Battle_targetResource(command._TargetResourceType,attack,target,btd._IsCure);
            var actNum= btr.Action();
            if (btd._IsCure) _battleAction_cure?.Invoke(target._myCharData, actNum);
            else _battleAction_damage?.Invoke(target._myCharData, actNum);
            if (!target.IsAlive()) _battleAction_defeat?.Invoke(target._myCharData);

            //if (btd._IsCure)
            //{
            //    var cure = attack;
            //    target.SetCure(cure);
            //    _battleAction_cure?.Invoke(target._myCharData, cure);
            //}
            //else
            //{
            //    var damage = target.CalcDamage(attack);
            //    target.SetDamage(damage);
            //    _battleAction_damage?.Invoke(target._myCharData, damage);
            //    if (!target.IsAlive()) _battleAction_defeat?.Invoke(target._myCharData);
            //}
        }
        _battleAction_endTurn?.Invoke();
        _battleCharQueue.Enqueue(next);
        PrepareNextTurn();
    }

    public void SetCharInput(string charName,string command)
    {
        var target = _battleCharQueue.Where(x => x._myCharData._name == charName).FirstOrDefault();
        _charInput = (command, target);
        _waitInput = false;
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
            battle.TurnAct();
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
