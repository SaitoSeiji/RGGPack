using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DBDInterface;

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

    public void TurnAction(BattleChar user,BattleChar inputTarget, ICommandData icommand, BattleController bt)
    {
        var command = icommand.GetCommandData();
        UseResource(user, icommand);

        var effectNum = CalcEffectNum(user, command);
        var btd = bt.GetCommandTargetDicide(icommand);
        var targetPool = btd.GetTargetPool();
        var targetList = btd.SelectTarget(targetPool, inputTarget);

        TrrigerEffect(effectNum, command, targetList, btd._IsCure, bt);
    }

    protected abstract void UseResource(BattleChar user, ICommandData command);
    protected abstract void TrrigerEffect(int attack, CommandData command, List<BattleChar> targetList, bool isCure, BattleController bt);
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

    protected override void TrrigerEffect(int attack, CommandData command, List<BattleChar> targetList, bool isCure, BattleController bt)
    {
        foreach (var target in targetList)
        {
            var btr = new Battle_targetResource(command._targetResourceType, attack, target, isCure);
            var actNum = btr.Action();

            //コールバックをこっちで呼んでるのはよくなさそう
            if (isCure) bt._battleAction_cure?.Invoke(target._myCharData, actNum);
            else bt._battleAction_damage?.Invoke(target._myCharData, actNum);
            if (!target.IsAlive()) bt._battleAction_defeat?.Invoke(target._myCharData);
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

    protected override void TrrigerEffect(int attack, CommandData command, List<BattleChar> targetList, bool isCure, BattleController bt)
    {
        foreach (var target in targetList)
        {
            var btr = new Battle_targetResource(command._targetResourceType, attack, target, isCure);
            var actNum = btr.Action();

            //コールバックをこっちで呼んでるのはよくなさそう
            if (isCure) bt._battleAction_cure?.Invoke(target._myCharData, actNum);
            else bt._battleAction_damage?.Invoke(target._myCharData, actNum);
            if (!target.IsAlive()) bt._battleAction_defeat?.Invoke(target._myCharData);
        }
    }
}
