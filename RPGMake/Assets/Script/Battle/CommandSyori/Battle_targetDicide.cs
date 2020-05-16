using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CommandEnums;

public class Battle_targetDicide
{
    
    TargetType _myTargetType;
    
    BattleChar _user;
    List<BattleChar> _friendList = new List<BattleChar>();
    List<BattleChar> _enemyList = new List<BattleChar>();

    public bool _IsCure
    {
        get
        {
            return CommandEnumAction.IsCure(_myTargetType);
        }
    }
    
    public Battle_targetDicide(TargetType target, BattleChar user, List<BattleChar> friends, List<BattleChar> enemys)
    {
        _myTargetType = target;
        _user = user;
        _friendList = friends;
        _enemyList = enemys;
    }
    //対象となりうるもののリストを返す
    public List<BattleChar> GetTargetPool()
    {
        var result= new List<BattleChar>();
        switch (_myTargetType)
        {
            case TargetType.SELF:
                result.Add(_user);
                break;
            case TargetType.FRIEND_SOLO:
            case TargetType.FRIEND_ALL:
            case TargetType.FRIEND_RANDOM:
                result.AddRange(_friendList);
                break;
            case TargetType.ENEMY_SOLO:
            case TargetType.ENEMY_ALL:
            case TargetType.ENEMY_RANDOM:
                result.AddRange(_enemyList);
                break;
        }

        return result;
    }

    //対象選択を入力でするかどうか
    public bool IsInputSelect()
    {
        switch (_myTargetType)
        {
            case TargetType.SELF:
            case TargetType.FRIEND_SOLO:
            case TargetType.ENEMY_SOLO:
                return true;
            case TargetType.NONE:
            case TargetType.FRIEND_ALL:
            case TargetType.FRIEND_RANDOM:
            case TargetType.ENEMY_ALL:
            case TargetType.ENEMY_RANDOM:
                return false;
            default:
                return false;
        }
    }
    //対象となる売るものを受け取って対象を選ぶ
    public List<BattleChar> SelectTarget(List<BattleChar> targetPool, BattleChar input=null)
    {
        if (IsInputSelect())//入力を受け取るもの
        {
            if (targetPool.Contains(input)) return new List<BattleChar>() { input};
            else//入力がないならエラー処理
            {
                Debug.Log("target is unselected");
                return null;
            }
        }
        var result = new List<BattleChar>();
        //入力を受け取らないもの
        switch (_myTargetType)
        {
            case TargetType.FRIEND_RANDOM:
            case TargetType.ENEMY_RANDOM:
                int rand= UnityEngine.Random.Range(0, targetPool.Count);
                result.Add(targetPool[rand]);
                break;
            case TargetType.FRIEND_ALL:
            case TargetType.ENEMY_ALL:
                result.AddRange(targetPool);
                break;
            default:
                return null;
        }
        return result;
    }

    
}
