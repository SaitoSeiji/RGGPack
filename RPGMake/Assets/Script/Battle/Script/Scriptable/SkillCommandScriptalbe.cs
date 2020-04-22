using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillCommandData
{
    public enum TARGET
    {
        NONE, SELF, ENEMY
    }
    [SerializeField] public string _skillId;
    [SerializeField] public string _skillName;
    [SerializeField] public TARGET _target;
    [SerializeField] public float _rate;
}

[CreateAssetMenu(fileName = "SkillCommand", menuName = "CharData/SkillCommand", order = 0)]
public class SkillCommandScriptalbe : ScriptableObject
{
    [SerializeField] SkillCommandData _skill;
    public SkillCommandData _SKill { get { return _skill; } }
}
