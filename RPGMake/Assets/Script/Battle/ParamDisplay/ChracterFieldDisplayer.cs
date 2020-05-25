using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DisallowMultipleComponent]
public class ChracterFieldDisplayer : MonoBehaviour
{
    [SerializeField] List<PlayerParamDisplay> _playerDisplayers = new List<PlayerParamDisplay>();
    [SerializeField] List<EnemyParamDisplay> _enemyDisplays = new List<EnemyParamDisplay>();

    public void SetData(CharcterField ct)
    {
        for (int i = 0; i < ct._playerList.Count; i++)
        {
            _playerDisplayers[i].SetChar(ct._playerList[i]);
            _playerDisplayers[i].Activate();
        }
        for (int i = 0; i < ct._enemyList.Count; i++)
        {
            _enemyDisplays[i].SetChar(ct._enemyList[i]);
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
}
