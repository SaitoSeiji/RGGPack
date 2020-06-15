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
                            try
                            {
                                bool flag = temp[1].Equals("true", StringComparison.OrdinalIgnoreCase);
                                _codeData.coalTerm._orMode = flag;
                                if (!flag && !temp[1].Equals("false", StringComparison.OrdinalIgnoreCase))//falseを明示したとき以外のfalse
                                {
                                    ThrowErrorLog(null, "", "データ内容に誤りがあります", name, $"term,<{con}>");
                                }
                            }catch(IndexOutOfRangeException e)
                            {
                                ThrowErrorLog(e, "", "文法に誤りがあります", name, $"term,<{con}>");
                            }
                        }
                        else
                        {
                            try
                            {
                                int num = int.Parse(temp[2]);
                                var datatype = DataMemberInspector.CreateDataType(temp[0]);
                                var hikaku = DataMemberInspector.CreateHikaku(temp[3]);
                                _codeData.coalTerm.AddTerm(temp[1], num, datatype, hikaku);
                            }
                            catch (Exception e) when (e is IndexOutOfRangeException ||
                                           e is InvalidOperationException ||
                                           e is FormatException)
                            {
                                ThrowErrorLog(e, "", "文法に誤りがあります", name, $"term,<{con}>");
                            }
                            catch (Exception e) when (e is ArgumentException)
                            {
                                ThrowErrorLog(e, "", "データ内容に誤りがあります", name, $"term,<{con}>");
                            }
                        }
                    }
                    break;
                case "next":
                    UpdateData_next(id, data);
                    break;
                default:
                    ThrowErrorLog(null,"","括弧の前のやつが不正な値です",name,data._head);
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
        if (string.IsNullOrEmpty(_codeData._nextEventName)) return;
        try
        {
            var nextevent = database.Where(x => x.name == _codeData._nextEventName).First();
            _codeData._nextEventCode = nextevent;
        }
        catch(InvalidOperationException e)
        {
            ThrowErrorLog(e, "", "存在しないイベント名です", name, "next,"+ _codeData._nextEventName);
        }
    }


    public void SyntaxCheck(string fileName)
    {
        var codeList = TextConverter.Convert(GetData()).ToList();
        CodeData tempcode = new EndCode();
        foreach (var code in codeList)
        {
            var codeData = tempcode.CreateCodeData(code, this);
            if (codeData == null)
            {
                ThrowErrorLog(null, fileName, "ヘッダーが不正な値です", name, code._head);
                break;
            }
            else tempcode = codeData;
        }
    }

    protected static void ThrowErrorLog(Exception e, string filename, string errorCode, string serchid, string information)
    {
        if (string.IsNullOrEmpty(filename)) filename = "eventData";
        Debug.LogError($"{filename}:{errorCode}:発生個所 id:{serchid} info:{information}\n" +
                       $"エラー内容:{e}");
    }

}

