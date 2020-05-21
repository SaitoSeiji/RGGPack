using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGEnums;

public static class BattleCommandDataFormatter
{
    public static string Format(SkillCommandData data)
    {
        string result = "";
        result = $"{data._skillName}\n" +
            $"対象:{CommandEnumAction.Target2String(data._target)}\n" +
            $"消費:{CommandEnumAction.Resource2String(data._useResourceType)} {ConverUseNum(data._useNum)}\n" +
            $"効果:{ConvertEffect(data)}";
        return result;
    }
    
    static string ConverUseNum(int useNum)
    {
        if (useNum > 0) return useNum.ToString();
        return "-";
    }

    static string ConvertEffect(SkillCommandData data)
    {
        bool iscure = CommandEnumAction.IsCure(data._target);
        switch (data._TargetResourceType)
        {
            case ResourceType.HP:
                if (iscure)  return "対象を回復";
                else return "対象にダメージ";
            case ResourceType.SP:
                if (iscure)  return "対象のSPを回復";
                else return "対象のSPにダメージ";
            default:
                return "";
        }
    }
    
}
