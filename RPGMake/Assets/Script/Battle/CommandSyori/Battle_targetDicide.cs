using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RPGEnums;

public static class Battle_targetDicide
{
    public static List<BattleChar> GetTargetPool(TargetType target, BattleChar user, List<BattleChar> friends, List<BattleChar> enemys)
    {
        var result= new List<BattleChar>();
        switch (target)
        {
            case TargetType.SELF:
                result.Add(user);
                break;
            case TargetType.FRIEND_SOLO:
            case TargetType.FRIEND_ALL:
            case TargetType.FRIEND_RANDOM:
                if (friends == null) return result;
                var friends_alive = friends.Where(x =>x!=null&& x.IsAlive()).ToList();
                if (friends_alive == null) return result;
                result.AddRange(friends_alive);
                break;
            case TargetType.ENEMY_SOLO:
            case TargetType.ENEMY_ALL:
            case TargetType.ENEMY_RANDOM:
                if (enemys == null) return result;
                var enemy_alive = enemys.Where(x => x != null && x.IsAlive()).ToList();
                if (enemy_alive == null) return result;
                result.AddRange(enemy_alive);
                break;
        }

        return result;
    }

    //対象選択を入力でするかどうか
    public static bool IsInputSelect(TargetType target)
    {
        switch (target)
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
    public static List<BattleChar> SelectTarget(TargetType target, List<BattleChar> targetPool, BattleChar input=null)
    {
        if (IsInputSelect(target))//入力を受け取るもの
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
        switch (target)
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
