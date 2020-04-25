using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [SerializeField,NonEditable]BattleUIState _battleUIState;
    BattleUIState _nextUIState;

    //キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] UIBase _commandUI;
    [SerializeField] UIBase _textUI;
    [SerializeField] TextDisplayer _battleTextDisplayer;

    [SerializeField] CharParamDisplay _playerParam;
    [SerializeField] List<CharParamDisplay> _enemyParams;
    UIBase _selfUI;
    public UIBase _SelfUI
    {
        get
        {
            if(_selfUI==null)_selfUI = GetComponent<UIBase>();
            return _selfUI;
        }
    }
    //==============================================
    List<string> _displayText;

    string temp_command;
    string temp_target;
    

    private void OnDisable()
    {
        _nextUIState = BattleUIState.None;
        _battleUIState = BattleUIState.None;
    }

    private void Update()
    {
        if (BattleController_mono.Instance.battle == null) return;
        switch (_battleUIState)
        {
            case BattleUIState.Switch:
                
                if (_displayText!=null)
                {
                    SwichUIState(BattleUIState.DisplayText);
                    DisplayText(_displayText);
                }
                else if (BattleController_mono.Instance.battle._waitInput)
                {
                    SwichUIState(BattleUIState.WaitInput);
                }
                else if (BattleController_mono.Instance.IsEnd())
                {
                    _playerParam.EndChar();
                    foreach(var data in _enemyParams)
                    {
                        data.EndChar();
                    }
                    SwichUIState(BattleUIState.None);
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
                            _SelfUI.AddUI(_commandUI);
                        }
                        else EndSwichUIState();
                        break;
                    case BattleUIState.DisplayText:
                        if (_textUI._NowUIState == UIBase.UIState.CLOSE)
                        {
                            _SelfUI.AddUI(_textUI);
                        }
                        else EndSwichUIState();
                        break;
                    case BattleUIState.None:
                        if (_SelfUI._NowUIState == UIBase.UIState.ACTIVE)
                        {
                            _SelfUI.CloseUI(_SelfUI);
                        }
                        break;
                }
                break;
            case BattleUIState.WaitInput:
                if (_SelfUI._NowUIState==UIBase.UIState.ACTIVE)
                {
                    _battleUIState = BattleUIState.Switch;
                    BattleController_mono.Instance.SetCharInput(temp_target, temp_command);
                }
                break;
            case BattleUIState.DisplayText:
                if (!_battleTextDisplayer._readNow)
                {
                    _playerParam.SyncData();
                    foreach(var data in _enemyParams)
                    {
                        data.SyncData();
                    }
                    _battleUIState = BattleUIState.Switch;
                    CloseText();
                }
                break;
            case BattleUIState.Process:
                BattleController_mono.Instance.Next();
                _battleUIState = BattleUIState.Switch;
                break;
        }
    }


    public void EndCommand(string command,string target,UIBase coalSelf)
    {
        temp_command = command;
        temp_target = target;
        coalSelf.CloseToUI(_SelfUI);
    }
    
    void DisplayText(List<string> input)
    {
        _battleTextDisplayer.SetTextData(input);
        _battleTextDisplayer.StartEvent();
    }

    void CloseText()
    {
        _displayText = null;
        _textUI.CloseToUI(_SelfUI);
    }

    public void AddDisplayText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (_displayText == null)
        {
            _displayText = new List<string>();
        }
        var splited = text.Split('$');
        _displayText.AddRange(splited);
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
        _playerParam.SetChar( BattleController_mono.Instance.battle._player);
        var enemys = BattleController_mono.Instance.battle._enemys;
        for (int i = 0; i < enemys.Count; i++)
        {
            _enemyParams[i].SetChar(enemys[i]);
        }
    }
    
}
