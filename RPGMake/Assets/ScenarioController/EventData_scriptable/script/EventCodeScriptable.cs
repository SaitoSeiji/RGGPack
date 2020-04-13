using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EventData/Create EventCode", fileName = "EventCode")]
public class EventCodeScriptable : ScriptableObject
{
    //[SerializeField,TextArea(0,10)] List<string> data;
    [SerializeField, TextArea(0, 100)] string _data;
    [SerializeField] EventCodeScriptable _nextEventCode;
    [SerializeField]EventCodeScriptablesTerm coalTerm;
    protected Dictionary<string, string> _flashData = new Dictionary<string, string>();

    public string GetData()
    {
        return _data;
    }
    public virtual bool CoalEnable()
    {
        return coalTerm.CoalEnable();
    }

    public virtual EventCodeScriptable GetNextCode()
    {
        return _nextEventCode;
    }

    //一時的に登録するデータ
    public void SetFlashData(string key,string data)
    {
        _flashData[key] = data;
    }
}

