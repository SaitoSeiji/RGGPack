using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIController : SingletonMonoBehaviour<BattleUIController>
{
    public enum BattleUIState
    {
        None,
        Switch,
        SwitchWait,
        WaitInput,
        DisplayText,
        Process
    }
    [SerializeField]BattleUIState _battleUIState;
    BattleUIState _nextUIState;
    [SerializeField] UIBase _commandUI;
    [SerializeField] UIBase _textUI;
    [SerializeField] TextDisplayer _battleTextDisplayer;
    UIBase _selfUI;
    [SerializeField] BattleController_mono bcmono;

    List<string> _displayText;

    string temp_command;
    string temp_target;

    private void Start()
    {
        _selfUI = GetComponent<UIBase>();
    }

    private void Update()
    {
        if (bcmono.battle == null) return;
        switch (_battleUIState)
        {
            case BattleUIState.Switch:
                
                if (_displayText!=null)
                {
                    SwichUIState(BattleUIState.DisplayText);
                    DisplayText(_displayText);
                }
                else if (bcmono.battle._waitInput)
                {
                    SwichUIState(BattleUIState.WaitInput);
                }
                else if (bcmono.IsEnd())
                {
                    _battleUIState = BattleUIState.None;
                }
                else
                {
                    _battleUIState = BattleUIState.Process;
                }
                break;
            //切り替え時にUIのAdd等が入る場合、一回でうまくいかない場合があるので、成功するまで繰り返した後
            //stateの遷移を行っている
            case BattleUIState.SwitchWait:
                switch (_nextUIState)
                {
                    case BattleUIState.WaitInput:
                        if (_commandUI._NowUIState == UIBase.UIState.CLOSE)
                        {
                            _selfUI.AddUI(_commandUI);
                        }
                        else EndSwichUIState();
                        break;
                    case BattleUIState.DisplayText:
                        if (_textUI._NowUIState == UIBase.UIState.CLOSE)
                        {
                            _selfUI.AddUI(_textUI);
                        }
                        else EndSwichUIState();
                        break;
                }
                break;
            case BattleUIState.WaitInput:
                if (_selfUI._NowUIState==UIBase.UIState.ACTIVE)
                {
                    _battleUIState = BattleUIState.Switch;
                    bcmono.SetCharInput(temp_target, temp_command);
                }
                break;
            case BattleUIState.DisplayText:
                if (!_battleTextDisplayer._readNow)
                {
                    _battleUIState = BattleUIState.Switch;
                    CloseText();
                }
                break;
            case BattleUIState.Process:
                bcmono.Next();
                _battleUIState = BattleUIState.Switch;
                break;
        }
    }


    public void EndCommand(string command,string target,UIBase coalSelf)
    {
        temp_command = command;
        temp_target = target;
        coalSelf.CloseToUI(_selfUI);
    }
    
    void DisplayText(List<string> input)
    {
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
        if (string.IsNullOrEmpty(text)) return;
        if (_displayText == null)
        {
            _displayText = new List<string>();
        }
        _displayText.Add(text);
    }


    void SwichUIState(BattleUIState state)
    {
        _nextUIState = state;
        _battleUIState = BattleUIState.SwitchWait;
    }

    void EndSwichUIState()
    {
        _battleUIState = _nextUIState;
        _nextUIState = BattleUIState.None;
    }

    public void StartBattle()
    {
        _battleUIState = BattleUIState.Switch;
    }
}
