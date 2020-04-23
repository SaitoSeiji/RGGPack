using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySetData
{
    public string _setId;
    public List<BattleCharScriptable> _charList;
}

[CreateAssetMenu(fileName = "EnemySet", menuName = "CharData/EnemySet", order = 0)]
public class EnemySetScriptable : ScriptableObject
{
    public EnemySetData _enemySetData;
}
