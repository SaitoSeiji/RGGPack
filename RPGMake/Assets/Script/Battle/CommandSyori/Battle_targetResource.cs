using CommandEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Battle_targetResource 
{

    ResourceType _myResourceType;
    int _attackNum;
    bool _isCure;
    BattleChar _myChar;
    public Battle_targetResource(ResourceType resource, int attack, BattleChar target,bool iscure)
    {
        _myResourceType = resource;
        _attackNum = attack;
        _myChar = target;
        _isCure = iscure;
    }

    /// <summary>
    /// 対象に指定された数値の計算をする処理
    /// </summary>
    /// <returns>発生した効果の値</returns>
    public int Action()
    {
        switch (_myResourceType)
        {
            case ResourceType.HP:
                if (_isCure)
                {
                    _myChar.SetCure(_attackNum);
                    return _attackNum;
                }
                else
                {
                    var damage = _myChar.CalcDamage(_attackNum);
                    _myChar.SetDamage(damage);
                    return damage;
                }
            case ResourceType.SP:
                if (_myChar is PlayerChar)
                {
                    var target = (PlayerChar)_myChar;
                    if (_isCure)
                    {
                        target.CureSP(_attackNum);
                        return _attackNum;
                    }
                    else
                    {
                        target.UseSP(_attackNum);
                        return _attackNum;
                    }
                }
                else return 0;
            default:
                return 0;
        }
    }
}
