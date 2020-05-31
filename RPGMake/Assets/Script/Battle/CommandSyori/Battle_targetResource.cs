using RPGEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Battle_targetResource 
{
    
    /// <summary>
    /// 対象に指定された数値の計算をする処理
    /// </summary>
    /// <returns>発生した効果の値</returns>
    public static int Action(ResourceType resource, int attack, BattleChar target, bool iscure)
    {
        switch (resource)
        {
            case ResourceType.HP:
                if (iscure)
                {
                    target.SetCure(attack);
                    return attack;
                }
                else
                {
                    var damage = target.CalcDamage(attack);
                    target.SetDamage(damage);
                    return damage;
                }
            case ResourceType.SP:
                if (target is PlayerChar)
                {
                    var targetPl = (PlayerChar)target;
                    if (iscure)
                    {
                        targetPl.CureSP(attack);
                        return attack;
                    }
                    else
                    {
                        targetPl.UseSP(attack);
                        return attack;
                    }
                }
                else return 0;
            default:
                return 0;
        }
    }

    public static bool IsUseAble(ResourceType resource, bool iscure, BattleChar target)
    {
        if (!iscure||!(target is PlayerChar)) return true;
        var targetPl = (PlayerChar)target;
        switch (resource)
        {
            case ResourceType.HP:
                return target._nowHp < target._maxHp;
            case ResourceType.SP:
                return targetPl._nowSP < targetPl._maxSP;
            default:
                return true;
        }
    }
}
