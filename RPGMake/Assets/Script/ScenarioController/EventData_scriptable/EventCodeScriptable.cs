using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class EventCodeData
{
    [SerializeField, TextArea(0, 100)]public string _text;
    [SerializeField] public string _nextEventName;
    [SerializeField]public EventCodeScriptable _nextEventCode;
    [SerializeField]public EventCodeScriptablesTerm coalTerm=new EventCodeScriptablesTerm();

    
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

    public void UpdateData(string id, List<EventDataOperater.ConvertedText> dataSet)
    {
        foreach(var data in dataSet)
        {
            switch (data._head)
            {
                case "text":
                    _codeData._text = string.Join("\n",data._content);
                    break;
                case "term":
                    _codeData.coalTerm.ResetTerm();
                    foreach (var con in data._content)
                    {
                        var temp = con.Split(' ');
                        if (temp[0].Equals("ormode", StringComparison.OrdinalIgnoreCase))//ormodeの設定
                        {
                            bool flag = temp[1].Equals("true", StringComparison.OrdinalIgnoreCase);
                            _codeData.coalTerm._orMode = flag;
                        }
                        else
                        {
                            int num = int.Parse(temp[2]);
                            var hikaku = DataMemberInspector.CreateHikaku(temp[3]);
                            _codeData.coalTerm.AddTerm(temp[0], temp[1], num, hikaku);
                        }
                    }
                    break;
                case "next":
                    UpdateData_next(id, data);
                    break;
            }
        }
    }

    protected virtual void UpdateData_next(string id, EventDataOperater.ConvertedText data )
    {
        _codeData._nextEventName = data._content[0];
    }

    public virtual void UpdateNextEvent(List<EventCodeScriptable> database)
    {
        var nextevent = database.Where(x => x.name == _codeData._nextEventName).FirstOrDefault();
        _codeData._nextEventCode = nextevent;
    }
}

