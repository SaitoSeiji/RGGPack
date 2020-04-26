using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EventDB",menuName = "DataBases/DataBase/EventDB",order = 0)]
public class EventDB : ScriptableObject
{
    [SerializeField]public List<EventCodeScriptable> _scriptableList=new List<EventCodeScriptable>();

    public void UpdateNextEvent()
    {
        foreach(var data in _scriptableList)
        {
            data.UpdateNextEvent(_scriptableList);
        }
    }
}
