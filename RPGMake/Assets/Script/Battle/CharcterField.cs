using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharcterField
{
    public List<PlayerChar> _playerList { get; private set; } = new List<PlayerChar>();
    public List<EnemyChar> _enemyList { get; private set; } = new List<EnemyChar>();

    public CharcterField(List<PlayerChar> playerList, List<EnemyChar> enemyList)
    {
        _playerList = playerList;
        _enemyList = enemyList;
    }

    public List<BattleChar> GetFriend(BattleChar target)
    {
        if (_playerList.Contains(target)) return _playerList.Select(x=>(BattleChar)x ).ToList();
        else  return _enemyList.Select(x => (BattleChar)x).ToList();
    }
    public List<BattleChar> GetEnemy(BattleChar target)
    {
        if (_playerList.Contains(target)) return _enemyList.Select(x => (BattleChar)x).ToList();
        else return _playerList.Select(x => (BattleChar)x).ToList();
    }

    public bool AllDead(List<BattleChar> check)
    {
        foreach(var c in check)
        {
            if (c.IsAlive()) return false;
        }
        return true;
    }

    public void SetRival()
    {
        foreach(var pl in _playerList)
        {
            foreach(var ene in _enemyList)
            {
                ene.AddRaival(pl);
                pl.AddRaival(ene);
            }
        }
    }

    #region name
    //複数同名モンスターがいるときに固有名にする AとかBとか
    //無駄に長いので短くできそう
    public static void SetUniqueName(List<EnemyChar> enemys)
    {
        var temp = new List<EnemyChar>(enemys);
        while (temp.Count > 0)
        {

            string targetName = temp[0]._myCharData._name;
            var targets = temp.Where(x => x._myCharData._name == targetName).ToArray();
            //名前がかぶっているならユニークネームにする
            if (targets.Length > 1)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i]._myCharData._name += ConvertNum(i);
                }
            }
            //変えた分を消す
            foreach (var t in targets)
            {
                temp.Remove(t);
            }
        }
    }

    static string ConvertNum(int num)
    {
        switch (num)
        {
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
            case 4:
                return "E";
        }
        return "";
    }
    #endregion

    public static List<PlayerChar> GetParty()
    {
        var db=SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>();
        return db.Select(x => new PlayerChar(x)).ToList();
    }
}
