using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIController : SingletonMonoBehaviour<BattleUIController>
{
    public enum BattleUIState
    {
        None,
        WaitInput,
        DisplayText
    }
    [SerializeField]BattleUIState _battleUIState;
    [SerializeField] UIBase _commandUI;
    [SerializeField] UIBase _textUI;
    [SerializeField] TextDisplayer _battleTextDisplayer;
    UIBase _selfUI;
    [SerializeField] BattleController_mono bcmono;

    List<string> _displayText;

    private void Start()
    {
        _selfUI = GetComponent<UIBase>();
    }

    private void Update()
    {
        if (bcmono.battle == null) return;
        switch (_battleUIState)
        {
            case BattleUIState.None:
                if (bcmono.battle._waitInput)
                {
                    _battleUIState = BattleUIState.WaitInput;
                    OpenCommandUI();
                }else if (_displayText!=null)
                {
                    _battleUIState = BattleUIState.DisplayText;
                    DisplayText(_displayText);
                    _selfUI.AddUI(_textUI);
                }
                else
                {
                    bcmono.Next();
                }
                break;
            case BattleUIState.WaitInput:
                if (!bcmono.battle._waitInput)
                {
                    _battleUIState = BattleUIState.None;
                }
                break;
            case BattleUIState.DisplayText:
                if (!_battleTextDisplayer._readNow)
                {
                    _battleUIState = BattleUIState.None;
                    CloseText();
                }else if (_textUI._NowUIState == UIBase.UIState.CLOSE)
                {
                    _selfUI.AddUI(_textUI);
                }
                break;
        }
    }

    void OpenCommandUI()
    {
        _selfUI.AddUI(_commandUI);
    }

    public void EndCommand(int command,int target,UIBase coalSelf)
    {
        coalSelf.CloseToUI(_selfUI);
        bcmono.SetCharInput(target, command);
    }
    
    void DisplayText(List<string> input)
    {
        //Debug.Log(_selfUI._NowUIState);
        _battleTextDisplayer.SetTextData(input);
        _battleTextDisplayer.StartEvent();
    }

    void CloseText()
    {
        _displayText = null;
        _textUI.CloseToUI(_selfUI);
    }

    public void AddDisplayText(string text)
    {
        if (_displayText == null)
        {
            _displayText = new List<string>();
        }
        _displayText.Add(text);
    }

}
