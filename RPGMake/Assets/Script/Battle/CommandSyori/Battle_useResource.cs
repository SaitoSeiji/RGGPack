using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommandEnums;

//攻撃するときに使用するものについての処理
public class Battle_useResource
{
    //key は　resource
    
    ResourceType _myResourceType;
    int _useNum;
    PlayerChar _myPl;

    public Battle_useResource(ResourceType resource,int num, PlayerChar user)
    {
        _myResourceType = resource;
        _useNum = GetUseNum(num);
        _myPl = user;
    }
    #region public関数
    public bool IsUseable()
    {
        return  GetTargetResource()>= _useNum;
    }

    public void Use()
    {
        SetTargetResource(_useNum);
    }
    #endregion
    #region private関数
    int GetUseNum(int rowNum)
    {
        switch (_myResourceType)
        {
            case ResourceType.HP:
            case ResourceType.SP:
                return rowNum;
            case ResourceType.NONE:
            default:
                return 0;
        }
    }

    int GetTargetResource()
    {
        switch (_myResourceType)
        {
            case ResourceType.NONE:
                return 0;
            case ResourceType.HP:
                return _myPl._nowHp;
            case ResourceType.SP:
                return _myPl._nowSP;
            default:
                return -1;
        }
    }
     void SetTargetResource(int num)
    {
        switch (_myResourceType)
        {
            case ResourceType.NONE:
                break;
            case ResourceType.HP:
                _myPl.SetDamage(num);
                break;
            case ResourceType.SP:
                _myPl.UseSP(num);
                break;
            default:
                break;
        }
    }
    #endregion

}
