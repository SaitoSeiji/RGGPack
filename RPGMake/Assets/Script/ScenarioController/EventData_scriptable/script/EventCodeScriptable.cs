using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventCodeData
{
    [SerializeField, TextArea(0, 100)]public string _text;
    [SerializeField]public EventCodeScriptable _nextEventCode;
    [SerializeField]public EventCodeScriptablesTerm coalTerm;


    public Dictionary<string, string> _flashData = new Dictionary<string, string>();
    //一時的に登録するデータ
    public void SetFlashData(string key, string data)
    {
        _flashData[key] = data;
    }
}

[CreateAssetMenu(menuName = "EventData/Create EventCode", fileName = "EventCode")]
public class EventCodeScriptable : ScriptableObject
{
    //[SerializeField,TextArea(0,10)] List<string> data;
    [SerializeField]public EventCodeData _codeData=new EventCodeData();
    string _addText = "";
    
    public void AddData(string code)
    {
        _addText = code;
    }

    public string GetData()
    {
        var result = _addText.Clone() + _codeData._text;
        return result;
    }
    public virtual bool CoalEnable()
    {
        return _codeData.coalTerm.CoalEnable();
    }

    public virtual EventCodeScriptable GetNextCode()
    {
        return _codeData._nextEventCode;
    }

}

