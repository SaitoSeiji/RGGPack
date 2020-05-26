using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DBDInterface;

[DisallowMultipleComponent]
public class ChracterFieldDisplayer : MonoBehaviour
{
    [SerializeField] List<PlayerParamDisplay> _playerDisplayers = new List<PlayerParamDisplay>();
    [SerializeField] List<EnemyParamDisplay> _enemyDisplays = new List<EnemyParamDisplay>();
    CharcterField _mycf;

    public void SetData(CharcterField cf)
    {
        _mycf = cf;
        for (int i = 0; i < cf._playerList.Count; i++)
        {
            _playerDisplayers[i].SetChar(cf._playerList[i]);
            _playerDisplayers[i].Activate();
        }
        for (int i = 0; i < cf._enemyList.Count; i++)
        {
            _enemyDisplays[i].SetChar(cf._enemyList[i]);
            _enemyDisplays[i].Activate();
        }
    }

    public AbstractParamDisplay GetParamDisplayer(SavedDBData_char chars)
    {
        string charname = chars._name;
        var plNameList = _playerDisplayers.Select(x => x._mycharData._myCharData._name);
        var eneNameList = _enemyDisplays.Select(x => x._mycharData._myCharData._name);
        if (plNameList.Contains(charname))
        {
            return _playerDisplayers.Where(x => x._mycharData._myCharData._name == charname).First();
        }else if (eneNameList.Contains(charname))
        {
            return _enemyDisplays.Where(x => x._mycharData._myCharData._name == charname).First();
        }
        else
        {
            return null;
        }
    }

    public void SyncParam_pl()
    {
        _playerDisplayers.Where(x=>x.gameObject.activeInHierarchy).ToList().ForEach(x => x.SyncDisply());
    }
    public void SyncParam_enemy()
    {
        _enemyDisplays.Where(x => x.gameObject.activeInHierarchy).ToList().ForEach(x => x.SyncDisply());
    }
    #region item
    public void UseItem_menu(ItemData item,PlayerChar target)
    {
        var strtegy = CommandStrategy.GetStrategy(item);
        var friends = _playerDisplayers.Select(x=>(BattleChar)x._mycharData).ToList();
        var user = new PlayerChar(SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>()[0]);

        strtegy.TurnAction(user, target,item, friends: friends);
        SaveDataController.Instance.SetData<PlayerDB, SavedDBData_player>(target._myCharData);
    }

    public List<PlayerChar> GetTargetPool(ICommandData targetIntarface)
    {
        var commandData = targetIntarface.GetCommandData();
        var user = _playerDisplayers[0]._mycharData;
        return Battle_targetDicide.GetTargetPool(commandData._target,user , _mycf.GetFriend(user), _mycf.GetEnemy(user))
            .Select(x=>(PlayerChar)x).ToList();
    }
    #endregion
}
