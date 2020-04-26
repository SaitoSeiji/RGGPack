using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleUIController : SingletonMonoBehaviour<BattleUIController>
{
    public enum BattleState
    {
        None,
        First,
        Battle,
        Result,
        Close
    }
    [SerializeField, NonEditable] BattleState _battleState;

    public enum BattleUIState
    {
        None,
        Switch,

        StateStart,
        StateEnd,

        WaitInput,
        DisplayText,
        Process
    }
    [SerializeField,NonEditable]BattleUIState _nowUIState;
    BattleUIState _nextUIState;

    //キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] UIBase _baseUI;
    [SerializeField] UIBase _commandUI;
    [SerializeField] UIBase _textUI;
    [SerializeField] TextDisplayer _battleTextDisplayer;

    [SerializeField,Space] CharParamDisplay _playerParam;
    [SerializeField] List<CharParamDisplay> _enemyParams;
    public UIBase _BaseUI
    {
        get
        {
            return _baseUI;
        }
    }
    //==============================================
    List<string> _displayText;
    BattleController _battle { get {return BattleController_mono.Instance.battle; } }

    string temp_command;
    string temp_target;
    

    private void OnDisable()
    {
        _nextUIState = BattleUIState.None;
        _nowUIState = BattleUIState.None;
        _battleState = BattleState.None;
    }

    private void Update()
    {
        BattleStateUpdate();
        UIStateUpdate();
    }
    void BattleStateUpdate()
    {
        if (_nowUIState != BattleUIState.Switch) return;
        switch (_battleState)
        {
            case BattleState.First:
                {
                    var log = _battle.GetLog("encount");
                    AddDisplayText(log);
                    ChengeUIState(BattleUIState.DisplayText);
                    _battleState = BattleState.Battle;
                }
                break;
            case BattleState.Battle:
                {
                    if (_battle.IsEnd())
                    {
                        _battleState = BattleState.Result;
                    }
                    else
                    {
                        if (_battle._waitInput)
                        {
                            ChengeUIState(BattleUIState.WaitInput);
                        }
                        else
                        {
                            BattleController_mono.Instance.Next();
                            var log = _battle.GetLog("command") + _battle.GetLog("damage") + _battle.GetLog("defeat");
                            AddDisplayText(log);
                            ChengeUIState(BattleUIState.DisplayText);
                        }
                    }
                }
                break;
            case BattleState.Result:
                {
                    var log = _battle.GetLog("end");
                    AddDisplayText(log);
                    ChengeUIState(BattleUIState.DisplayText);
                    _battleState = BattleState.Close;
                }
                break;
            case BattleState.Close:
                {
                    ChengeUIState(BattleUIState.None);
                    _battleState = BattleState.None;
                }
                break;
        }
    }

    void UIStateUpdate()
    {
        switch (_nowUIState)
        {
            case BattleUIState.Switch:
                break;
            //切り替え時にUIのAdd等が入る場合、一回でうまくいかない場合があるので、成功するまで繰り返した後
            //stateの遷移を行っている
            case BattleUIState.StateStart:
                switch (_nextUIState)
                {
                    case BattleUIState.WaitInput:
                        if (_commandUI._NowUIState == UIBase.UIState.CLOSE)
                        {
                            _BaseUI.AddUI(_commandUI);
                        }
                        else EndChengeUIState();
                        break;
                    case BattleUIState.DisplayText:
                        if (_textUI._NowUIState == UIBase.UIState.CLOSE)
                        {
                            _BaseUI.AddUI(_textUI);
                        }
                        else
                        {
                            DisplayText(_displayText);
                            EndChengeUIState();
                        }
                        break;
                    case BattleUIState.None:
                        if (_BaseUI._NowUIState == UIBase.UIState.ACTIVE)
                        {
                            _BaseUI.CloseUI(_BaseUI);
                        }
                        else EndChengeUIState();
                        break;
                    default:
                        EndChengeUIState();
                        break;
                }
                break;
            case BattleUIState.StateEnd://閉じるまで閉じ続ける
                switch (_nextUIState)
                {
                    case BattleUIState.DisplayText:
                        if (_textUI._NowUIState == UIBase.UIState.ACTIVE)
                        {
                            CloseText();
                        }else EndEndUIState();
                        break;
                    default:
                        EndEndUIState();
                        break;
                }
                break;
            case BattleUIState.WaitInput:
                if (_BaseUI._NowUIState == UIBase.UIState.ACTIVE)
                {
                    BattleController_mono.Instance.SetCharInput(temp_target, temp_command);
                    EndUIState(BattleUIState.WaitInput);
                }
                break;
            case BattleUIState.DisplayText:
                if (!_battleTextDisplayer._readNow)
                {
                    _playerParam.SyncData();
                    foreach (var data in _enemyParams)
                    {
                        data.SyncData();
                    }
                    EndUIState(BattleUIState.DisplayText);
                }
                break;
        }
    }

    //void UIStateUpdate()
    //{
    //    switch (_nowUIState)
    //    {
    //        case BattleUIState.Switch:
    //            if (_displayText != null)
    //            {
    //                ChengeUIState(BattleUIState.DisplayText);
    //                DisplayText(_displayText);
    //            }
    //            else if (BattleController_mono.Instance.battle._waitInput)
    //            {
    //                ChengeUIState(BattleUIState.WaitInput);
    //            }
    //            else if (BattleController_mono.Instance.IsEnd())
    //            {
    //                AddDisplayText(_battle.GetLog("end"));
    //                DisplayText(_displayText);
    //                ChengeUIState(BattleUIState.DisplayText_last);
    //            }
    //            else
    //            {
    //                _nowUIState = BattleUIState.Process;
    //            }
    //            break;
    //        //切り替え時にUIのAdd等が入る場合、一回でうまくいかない場合があるので、成功するまで繰り返した後
    //        //stateの遷移を行っている
    //        case BattleUIState.StateStart:
    //            switch (_nextUIState)
    //            {
    //                case BattleUIState.WaitInput:
    //                    if (_commandUI._NowUIState == UIBase.UIState.CLOSE)
    //                    {
    //                        _BaseUI.AddUI(_commandUI);
    //                    }
    //                    else EndChengeUIState();
    //                    break;
    //                case BattleUIState.DisplayText_last:
    //                case BattleUIState.DisplayText:
    //                    if (_textUI._NowUIState == UIBase.UIState.CLOSE)
    //                    {
    //                        _BaseUI.AddUI(_textUI);
    //                    }
    //                    else EndChengeUIState();
    //                    break;
    //                case BattleUIState.None:
    //                    if (_textUI._NowUIState == UIBase.UIState.ACTIVE)
    //                    {
    //                        CloseText();
    //                    }
    //                    else if (_BaseUI._NowUIState == UIBase.UIState.ACTIVE)
    //                    {
    //                        _BaseUI.CloseUI(_BaseUI);
    //                    }
    //                    else EndChengeUIState();
    //                    break;
    //            }
    //            break;
    //        case BattleUIState.WaitInput:
    //            if (_BaseUI._NowUIState == UIBase.UIState.ACTIVE)
    //            {
    //                _nowUIState = BattleUIState.Switch;
    //                BattleController_mono.Instance.SetCharInput(temp_target, temp_command);
    //            }
    //            break;
    //        case BattleUIState.DisplayText:
    //            if (!_battleTextDisplayer._readNow)
    //            {
    //                _playerParam.SyncData();
    //                foreach (var data in _enemyParams)
    //                {
    //                    data.SyncData();
    //                }
    //                _nowUIState = BattleUIState.Switch;
    //                CloseText();
    //            }
    //            break;
    //        case BattleUIState.DisplayText_last:
    //            if (!_battleTextDisplayer._readNow)
    //            {
    //                _playerParam.EndChar();
    //                foreach (var data in _enemyParams)
    //                {
    //                    data.EndChar();
    //                }
    //                ChengeUIState(BattleUIState.None);
    //            }
    //            break;
    //        case BattleUIState.Process:
    //            BattleController_mono.Instance.Next();
    //            var st = _battle.GetLog("command") + _battle.GetLog("damage") + _battle.GetLog("defeat");
    //            AddDisplayText(st);
    //            //DisplayText(_displayText);
    //            _nowUIState = BattleUIState.Switch;
    //            break;
    //    }
    //}

    public void EndCommand(string command,string target,UIBase coalSelf)
    {
        temp_command = command;
        temp_target = target;
        coalSelf.CloseToUI(_BaseUI);
    }
    
    void DisplayText(List<string> input)
    {
        _battleTextDisplayer.SetTextData(input);
        _battleTextDisplayer.StartEvent();
    }

    void CloseText()
    {
        _displayText = null;
        _textUI.CloseToUI(_BaseUI);
    }

     void AddDisplayText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        if (_displayText == null)
        {
            _displayText = new List<string>();
        }
        var splited = text.Split('$');
        _displayText.AddRange(splited);
    }

    #region uistate
    void ChengeUIState(BattleUIState state)
    {
        _nextUIState = state;
        _nowUIState = BattleUIState.StateStart;
    }

    void EndChengeUIState()
    {
        _nowUIState = _nextUIState;
        _nextUIState = BattleUIState.None;
    }

    void EndUIState(BattleUIState state)
    {
        _nextUIState = state;
        _nowUIState = BattleUIState.StateEnd;
    }

    void EndEndUIState()
    {
        _nowUIState = BattleUIState.Switch;
        _nextUIState = BattleUIState.None;
    }
    #endregion
    public void StartBattle()
    {
        _nowUIState = BattleUIState.Switch;
        _battleState = BattleState.First;
        _playerParam.SetChar( BattleController_mono.Instance.battle._player);
        var enemys = BattleController_mono.Instance.battle._enemys;
        for (int i = 0; i < enemys.Count; i++)
        {
            _enemyParams[i].SetChar(enemys[i]);
        }
    }

    public bool IsBattleNow()
    {
        return _nowUIState != BattleUIState.None;
    }
}
