using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DBDInterface;
using RPGEnums;
using System;

public abstract class CommandStrategy
{
    public static CommandStrategy GetStrategy(ICommandData data)
    {
        if (data is SkillCommandData)
        {
            return new SkillStrategy();
        }
        else if(data is ItemData)
        {
            return new ItemStrategy();
        }

        return null;
    }

    //引数多すぎてよくわからなくなる
    public void TurnAction(BattleChar user,BattleChar inputTarget, ICommandData icommand
        , Action<bool, bool, BattleChar, int> damageAction=null
        , List<BattleChar> friends=null,List<BattleChar> enemys=null)
    {
        var command = icommand.GetCommandData();
        UseResource(user, icommand);

        var effectNum = CalcEffectNum(user, command);
        var targetPool = Battle_targetDicide.GetTargetPool(command._target,user,friends,enemys);
        var targetList = Battle_targetDicide.SelectTarget(command._target,targetPool, inputTarget);
        
        TrrigerEffect(effectNum, command, targetList,
            CommandEnumAction.IsCure(command._target)
            , damageAction);
    }

    protected abstract void UseResource(BattleChar user, ICommandData command);
    protected abstract void TrrigerEffect(int attack, CommandData command, List<BattleChar> targetList, bool isCure,
        Action<bool, bool, BattleChar, int> damageAction);
    protected abstract int CalcEffectNum(BattleChar user, CommandData command);
}


public class SkillStrategy : CommandStrategy
{
    protected override void UseResource(BattleChar user, ICommandData icommand)
    {
        var command = icommand.GetCommandData();
        bool isPlayer = user is PlayerChar;
        if (isPlayer)
        {
            var pl = (PlayerChar)user;
            var bur = new Battle_useResource(command._useResourceType, command._useNum, pl);
            if (!bur.IsUseable()) return;
            bur.Use();
        }
    }

    protected override void TrrigerEffect(int attack, CommandData command, List<BattleChar> targetList, bool isCure, Action<bool, bool, BattleChar, int> damageAction)
    {
        foreach (var target in targetList)
        {
            var actNum = Battle_targetResource.Action(command._targetResourceType, attack, target, isCure);

            //コールバックをこっちで呼んでるのはよくなさそう
            damageAction?.Invoke(isCure,!target.IsAlive(),target,actNum);
        }
    }

    protected override int CalcEffectNum(BattleChar user, CommandData command)
    {
        return user.CalcurateAttack(command._effectNum);
    }
}
public class ItemStrategy : CommandStrategy
{
    protected override void UseResource(BattleChar user, ICommandData icommand)
    {
        var item = icommand as ItemData;
        var db = SaveDataController.Instance.GetDB_var<PartyDB, SavedDBData_party>()[0];
        db.ChengeItemNum(item,-1);
        SaveDataController.Instance.SetData<PartyDB, SavedDBData_party>(db);
    }

    protected override int CalcEffectNum(BattleChar user, CommandData command)
    {
        return (int) command._effectNum;
    }

    protected override void TrrigerEffect(int attack, CommandData command, List<BattleChar> targetList, bool isCure, Action<bool, bool, BattleChar, int> damageAction)
    {
        foreach (var target in targetList)
        {
            var actNum = Battle_targetResource.Action(command._targetResourceType, attack, target, isCure);

            damageAction?.Invoke(isCure, !target.IsAlive(), target, actNum);
        }
    }
}
