using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using System.Linq;

public class TextDisplayer:MonoBehaviour
{

    public static class TextReplacer
    {
        static string _replaceTextDataRGX = @"\[(.+?)\]";//[text]
        static string _kakkoNakaRGX = @"(.+?),(.+?)";//head,data
        static string _replaceTextData_reading_RGX = @"<(.+?)>";//<text> テキストを表示中に実行する

        public static string CheckReplace_before(string data)
        {
            return CheckReplace(data, _replaceTextDataRGX);
        }

        public static string CheckReplace_reading(string data)
        {
            if (data[0] != '<') return "";
            return CheckReplace(data, _replaceTextData_reading_RGX);
        }

        static string CheckReplace(string data,string rgxText)
        {
            var rgx = new Regex(rgxText);
            var match = rgx.Match(data);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        public static string ReplaceText_before(string data, string replace)
        {
            return ReplaceText(data, replace, _replaceTextDataRGX);
        }

        public static string ReplaceText_reading(string data)
        {
            return ReplaceText(data, "", _replaceTextData_reading_RGX);
        }

        static string ReplaceText(string data, string replace,string rgxText)
        {
            var rgx = new Regex(rgxText);
            return rgx.Replace(data, replace,1);
        }


        public static string GetReplaceContent_before(string target)
        {
            string result = target;
            var rgx = new Regex(_kakkoNakaRGX);
            var match = rgx.Match(target);
            if (match.Success)
            {
                string head = match.Groups[1].Value;
                string data = match.Groups[2].Value;
                switch (head)
                {
                    case "item":
                        result = ItemReplace(data);
                        break;
                }
            }
            else
            {

            }
            return result;
        }
        static string ItemReplace(string data)
        {
            switch (data)
            {
                case "getItem":
                    var key= EventCodeReadController.Instance.GetFlashData(data)[0];
                    EventCodeReadController.Instance.RemoveFlashData(data);
                    var target = SaveDataController.Instance.GetDB_var<ItemDB,SavedDBData_item>().Where(x => x._serchId == key).FirstOrDefault();
                    if (target == null) return "";
                    return target._displayName;
                    //return SaveDataController.Instance.GetText<ItemDB>(key.ToString(), "displayName");
            }
            return "";
        }
    }

    [SerializeField] Text _displayTextArea;
    [SerializeField] GameObject _textPanel;
    [SerializeField] int _maxDisplayLineCount = -1;//一度に表示する最大行数　-1だと制限なし

    Queue<string> _textData=new Queue<string>();
    public bool _readNow { get; private set; }
    string _nowTextData;

    Dictionary<string, Action> _textCommandList = new Dictionary<string, Action>();
    //textの表示パラメータ==============================
    [SerializeField] float charWaitTime;//1文字ごとの待ち時間
    
    WaitFlag _charWaitFlag;
    WaitFlag _endWaitFlag;
    bool waitSubmit = false;
    void Init()
    {
        if (_charWaitFlag != null) return;
        _charWaitFlag = new WaitFlag();
        _charWaitFlag.SetWaitLength(charWaitTime);
        _endWaitFlag = new WaitFlag();
        _endWaitFlag.SetWaitLength(charWaitTime);

        _displayTextArea.text = "";
    }

    private void Update()
    {
        if (waitSubmit)
        {
            if (SubmitInput()) waitSubmit = false;
            return;
        }
        if (_readNow)
        {
            if (_charWaitFlag._waitNow)
            {

            }
            else
            {
                if (IsEndNowText())
                {
                    if (SubmitInput())
                    {
                        if (IsEndAll())
                        {
                            EndEvent();
                        }
                        else
                        {
                            _nowTextData=GetNextRead();
                        }
                    }
                }
                else
                {
                    _nowTextData = CheckReplaceTargetText(_displayTextArea.text,_nowTextData);
                    _displayTextArea.text = GetUpdateText(_displayTextArea.text,_nowTextData);
                    _charWaitFlag.WaitStart();
                }

            }
        }
    }
    
    #region Public
    public void StartEvent()
    {
        Init();
        if (_readNow) return;
        _charWaitFlag.WaitStart();
        _readNow = true;
        _nowTextData = GetNextRead();
        _textPanel.SetActive(true);

        _textCommandList["w"] = () => waitSubmit = true;
    }

    public void SetTextData(List<string> textData)
    {
        foreach(var data in textData)
        {
            var divide = DivideForMaxLineCount(data);
            divide.ForEach(x => _textData.Enqueue(x));
        }
    }

    public void SetTextData(string textData)
    {
        var divide = DivideForMaxLineCount(textData);
        divide.ForEach(x => _textData.Enqueue(x));
    }
    public void SetTextData(string textData,string name)
    {
        var text = name + "\n「" +textData+ "」";
        var divide = DivideForMaxLineCount(text);
        divide.ForEach(x => _textData.Enqueue(x));
    }

    //数値以外で登録できるけど登録済みのやつを入れた時にはじかないとまずい　ex)<w>ははじかないとやばい
    public void AddTextAction(string num,Action act)
    {
        if (!_textCommandList.ContainsKey(num.ToString()))
        {
            _textCommandList[num]=act;
        }
        else
        {
            _textCommandList[num] += act;
        }
    }
    #endregion
    //_maxDisplayLineCount行ごとに分割する
    List<string> DivideForMaxLineCount(string textData)
    {
        if (_maxDisplayLineCount <= 0) return new List<string>() { textData };
        var result= new List<string>();
        string rgx = "(";//指定回数以上の改行を含んでいるか調べるrgx
        for(int i = 0; i < _maxDisplayLineCount; i++)
        {
            rgx += ".*?\n";
        }
        rgx += @")(\S)";
        string remainTarget = textData;
        var match = Regex.Match(remainTarget, rgx, RegexOptions.Multiline);
        while (match.Success)
        {
            var add = match.Groups[1].Value;
            result.Add(add);
            //remainTarget = match.Groups[2].Value;
            remainTarget = remainTarget.Substring(add.Length);
            match = Regex.Match(remainTarget, rgx, RegexOptions.Multiline);
        }
        result.Add(remainTarget);
        return result;
    }

    string GetNextRead()
    {
        _displayTextArea.text = "";
        var data= _textData.Dequeue();
        data = RepalceData(data);
        return data;
    }

    void EndEvent()
    {
        if (!_textPanel.activeInHierarchy) return;
        _displayTextArea.text = "";
        _textPanel.SetActive(false);
        _readNow = false;
        _textCommandList = new Dictionary<string, Action>();
    }

    bool SubmitInput()
    {
        return Input.GetKeyDown(KeyCode.Z);
    }

    bool IsEndAll()
    {
        return _textData.Count == 0;
    }

    bool IsEndNowText()
    {
        return _nowTextData == _displayTextArea.text;
    }

    //表示テキストの更新
    static string GetUpdateText(string displayText, string targetText)
    {
        if (displayText.Length >= targetText.Length) return displayText;
        string result = displayText;
        int nowCharCount = result.Length;
        result += targetText[nowCharCount];
        return result;
    }
    string CheckReplaceTargetText(string displayText, string targetText)
    {
        int nowCharCount = displayText.Length;
        string check = TextReplacer.CheckReplace_reading(targetText.Substring(nowCharCount));
        if (!string.IsNullOrEmpty(check))
        {
            _textCommandList[check]?.Invoke();
            targetText = TextReplacer.ReplaceText_reading(targetText);
        }
        return targetText;
    }
    #region repalace
    string RepalceData(string text)
    {
        var tempText = text;
        string check = TextReplacer.CheckReplace_before(tempText);
        while (!string.IsNullOrEmpty(check))
        {
            var replace = TextReplacer.GetReplaceContent_before(check);
            tempText = TextReplacer.ReplaceText_before(tempText, replace);
            check = TextReplacer.CheckReplace_before(tempText);
        }
        return tempText;
    }

    #endregion
    //test======================

    [ContextMenu("testDisplay")]
    public void TestDisplayer()
    {
        StartEvent();
    }
}
