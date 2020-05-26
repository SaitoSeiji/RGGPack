using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//コマンドなどで使用される列挙型
namespace RPGEnums
{
    //key は　target
    public enum TargetType
    {
        NONE = -1,
        SELF = 0,//(回復)
        FRIEND_SOLO = 1,//(回復)
        FRIEND_ALL = 2,//(回復)
        FRIEND_RANDOM = 3,//(回復)

        ENEMY_SOLO = 4,
        ENEMY_ALL = 5,
        ENEMY_RANDOM = 6
    }

    public enum ResourceType
    {
        NONE = 0,
        SP = 1,
        HP = 2,
    }
    public static class CommandEnumAction
    {
        public static string Target2String(TargetType type)
        {
            switch (type)
            {
                case TargetType.SELF:
                    return "自分";
                case TargetType.FRIEND_SOLO:
                    return "仲間一人";
                case TargetType.FRIEND_ALL:
                    return "仲間全体";
                case TargetType.FRIEND_RANDOM:
                    return "仲間ランダム１体";
                case TargetType.ENEMY_SOLO:
                    return "敵1体";
                case TargetType.ENEMY_ALL:
                    return "敵全体";
                case TargetType.ENEMY_RANDOM:
                    return "敵ランダム１体";
                case TargetType.NONE:
                default:
                    return "";
            }
        }

        public static bool IsCure(TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.SELF:
                case TargetType.FRIEND_SOLO:
                case TargetType.FRIEND_ALL:
                case TargetType.FRIEND_RANDOM:
                    return true;
                case TargetType.ENEMY_SOLO:
                case TargetType.ENEMY_ALL:
                case TargetType.ENEMY_RANDOM:
                case TargetType.NONE:
                default:
                    return false;
            }
        }

        public static string Resource2String(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.HP:
                    return "HP";
                case ResourceType.SP:
                    return "SP";
                default:
                    return "-";
            }
        }
    }
}




