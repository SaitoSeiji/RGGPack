using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/registar/Create FlagSaveData_registar", fileName = "FlagSaveData_registar")]
public class FlagSaveData_registar : ScriptableObject
{
    [SerializeField] public List<string> _flagSaveDataNameList;


}
