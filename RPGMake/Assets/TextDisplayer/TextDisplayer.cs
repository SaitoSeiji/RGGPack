using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TextDisplayer:SingletonMonoBehaviour<TextDisplayer>
{

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
                            _readNow = false;
                            _endWaitFlag.WaitStart();
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
        else
        {
            //読み込みが終了してから一瞬間をおいて、追加のテキストを入れることができる
            //無理やりすぎるので直したい
            if (!_endWaitFlag._waitNow)
            {
                EndEvent();
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
        return _textData.Dequeue();
    }


    void EndEvent()
    {
        if (!_textPanel.activeInHierarchy) return;
        _displayTextArea.text = "";
        _textPanel.SetActive(false);
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

    static string GetUpdateText(string displayText,string targetText)
    {
        string result = displayText;
        int nowCharCount = result.Length;
        result += targetText[nowCharCount];
        return result;

        //string nowDisplayText = _displayTextArea.text;
        //string nowData = _textData.Data[_nowTextDataIndex];
        //int nowCharCount = nowDisplayText.Length;

        //nowDisplayText += nowData[nowCharCount];
        //return nowDisplayText;
    }

    //test======================

    [ContextMenu("testDisplay")]
    public void TestDisplayer()
    {
        StartEvent();
    }
}
