using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TextDisplayer:SingletonMonoBehaviour<TextDisplayer>
{
    public static class TextReplacer
    {
        static string _replaceTextDataRGX = @"\[(.+)\]";//[text]
        static string _kakkoNakaRGX = @"(.+),(.+)";//head,data

        public static string CheckReplace(string data)
        {
            var rgx = new Regex(_replaceTextDataRGX);
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

        public static string ReplaceText(string data, string replace)
        {
            var rgx = new Regex(_replaceTextDataRGX);
            return rgx.Replace(data, replace);
        }


        public static string GetReplaceContent(string target)
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
                    return SaveDataController.Instance.GetText<ItemDB>(key.ToString(), "displayName");
            }
            return "";
        }
    }

    [SerializeField] Text _displayTextArea;
    [SerializeField] GameObject _textPanel;

    Queue<string> _textData=new Queue<string>();
    public bool _readNow { get; private set; }
    string _nowTextData;
    
    //textの表示パラメータ==============================
    [SerializeField] float charWaitTime;//1文字ごとの待ち時間
    
    WaitFlag _charWaitFlag;
    WaitFlag _endWaitFlag;

    private void Start()
    {
        _charWaitFlag = new WaitFlag();
        _charWaitFlag.SetWaitLength(charWaitTime);
        _endWaitFlag = new WaitFlag();
        _endWaitFlag.SetWaitLength(charWaitTime);

        _displayTextArea.text = "";
    }

    private void Update()
    {
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
                    _displayTextArea.text = GetUpdateText(_displayTextArea.text,_nowTextData);
                    _charWaitFlag.WaitStart();
                }

            }
        }
    }
    #region Public
    public void StartEvent()
    {
        if (_readNow) return;
        _charWaitFlag.WaitStart();
        _readNow = true;
        _nowTextData = GetNextRead();
        _textPanel.SetActive(true);
    }

    public void SetTextData(List<string> textData)
    {
        foreach(var data in textData)
        {
            _textData.Enqueue(data);
        }
    }

    public void SetTextData(string textData)
    {
        _textData.Enqueue(textData);
    }
    public void SetTextData(string textData,string name)
    {
        var text = name + "\n「" +textData+ "」";
        _textData.Enqueue(text);
    }
    #endregion


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
    static string GetUpdateText(string displayText,string targetText)
    {
        string result = displayText;
        int nowCharCount = result.Length;
        result += targetText[nowCharCount];
        return result;
    }
    #region repalace
    string RepalceData(string text)
    {
        var tempText = text;
        string check = TextReplacer.CheckReplace(tempText);
        while (!string.IsNullOrEmpty(check))
        {
            var replace = TextReplacer.GetReplaceContent(check);
            tempText = TextReplacer.ReplaceText(tempText, replace);
            check = TextReplacer.CheckReplace(tempText);
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
