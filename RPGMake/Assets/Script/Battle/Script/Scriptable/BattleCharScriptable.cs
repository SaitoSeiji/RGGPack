using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleCharData
{
    [SerializeField] public string _id;
    [SerializeField] public string _name;
    [SerializeField] public int _hp;
    [SerializeField] public int _attack;
    [SerializeField] public int _guard;
    public List<SkillCommandScriptalbe> _mySkillList = new List<SkillCommandScriptalbe>();

    public BattleCharData(int hp, int attack, int guard)
    {
        _hp = hp;
        _attack = attack;
        _guard = guard;
    }

    public BattleCharData(BattleCharData data)
    {
        _id = data._id;
        _name = data._name;
        _hp = data._hp;
        _attack = data._attack;
        _guard = data._guard;
        _mySkillList = data._mySkillList;
    }

    public BattleCharData Clone()
    {
        return new BattleCharData(this);
    }
}

[CreateAssetMenu(fileName = "BattleChar", menuName = "CharData/BattleChar", order = 0)]
public class BattleCharScriptable : ScriptableObject
{
    [SerializeField] BattleCharData _charData;
    public BattleCharData _CharData { get { return _charData.Clone(); } }
}
