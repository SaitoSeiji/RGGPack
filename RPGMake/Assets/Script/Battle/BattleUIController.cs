using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattleUIController : SingletonMonoBehaviour<BattleUIController>
{
    public enum BattleState
    {
        None,
        Battle,
        Close
    }
    [SerializeField, NonEditable] BattleState _battleState;

    public enum BattleUIState
    {
        None,
        NoUI,

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

    [SerializeField,Space] PlayerParamDisplay _playerParam;
    [SerializeField] List<EnemyParamDisplay> _enemyParams;
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
    Queue<Action> _uiActionQueue=new Queue<Action>();
    

    private void OnDisable()
    {
        _nextUIState = BattleUIState.None;
        _nowUIState = BattleUIState.None;
        _battleState = BattleState.None;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        BattleStateUpdate();
        UIStateUpdate();
    }
    void BattleStateUpdate()
    {
        if (_nowUIState != BattleUIState.NoUI) return;
        if (_uiActionQueue.Count > 0)
        {
            _uiActionQueue.Dequeue().Invoke();
            return;
        }
        switch (_battleState)
        {
            case BattleState.Battle:
                {
                    BattleController_mono.Instance.Next();
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
            case BattleUIState.NoUI:
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
                    EndUIState(BattleUIState.DisplayText);
                }
                break;
        }
    }
    
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
        _nowUIState = BattleUIState.NoUI;
        _nextUIState = BattleUIState.None;
    }
    #endregion
    public void SetUpCharData()
    {
        _nowUIState = BattleUIState.NoUI;
        _playerParam.SetChar( BattleController_mono.Instance.battle._player);
        _playerParam.Activate();
        var enemys = BattleController_mono.Instance.battle._enemys;
        for (int i = 0; i < enemys.Count; i++)
        {
            _enemyParams[i].SetChar(enemys[i]);
            _enemyParams[i].Activate();
        }
    }
    public void SetUpBattleDelegate(BattleController battle)
    {
        battle._battleAction_encount = EncountAction;
        battle._battleAction_waitInput = () =>{_uiActionQueue.Enqueue(()=> ChengeUIState(BattleUIState.WaitInput));};
        battle._battleAction_command = CommandAction;
        battle._battleAction_damage = DamageAction;
        battle._battleAction_cure = CureAction;
        battle._battleAction_defeat = DefeatAction;
        battle._battleAction_endTurn = EndTurnAction;
        battle._battleAction_end = EndBattleAction;
    }

    public bool IsBattleNow()
    {
        return _nowUIState != BattleUIState.None;
    }
    #region battleActionのデリゲート登録用関数
    void EncountAction()
    {
        _uiActionQueue.Enqueue(() =>
        {
            string log = "魔物が現れた";
            AddDisplayText(log);
            ChengeUIState(BattleUIState.DisplayText);
            _battleState = BattleState.Battle;
        }
        );
    }
    string _battlelogText = "";
    void CommandAction(SavedDBData_char user, SkillCommandData skilldata)
    {
        _uiActionQueue.Enqueue(() =>
        {
            _battlelogText = string.Format("{0}の{1}\n", user._name, skilldata._skillName);
            var target = GetParamDisplayer(user);
            target.SyncDisply();
        });
    }


    void DamageAction(SavedDBData_char chars, int damage)
    {
        _uiActionQueue.Enqueue(() =>
        {
            _battlelogText += string.Format("{0}は{1}のダメージ<{0}0>を受けた\n", chars._name, damage);
            string charname = chars._name.Clone().ToString();
            _battleTextDisplayer.AddTextAction(chars._name + "0", () =>
              {
                  //対象のcharにdamageActionするだけ
                  var target = GetParamDisplayer(chars);
                  if (target != null)
                  {

                      target.SyncDisply();
                      target.DamageAction();
                  }
              });

        });
    }

    void CureAction(SavedDBData_char chars, int damage)
    {
        _uiActionQueue.Enqueue(() =>
        {
            _battlelogText += string.Format("{0}は{1}回復<{0}1>\n", chars._name, damage);
            _battleTextDisplayer.AddTextAction(chars._name + "1", () =>
            {
                var target = GetParamDisplayer(chars);
                if(target!=null) target.SyncDisply();
            });
        });
    }

    void DefeatAction(SavedDBData_char chars)
    {
        _uiActionQueue.Enqueue(() =>
        {
            if (chars == null) return;
            _battlelogText += string.Format("{0}は倒れた<{0}2>\n", chars._name);
            _battleTextDisplayer.AddTextAction(chars._name + "2", () =>
              {
            //対象のcharにDeadActionするだけ
            if (chars._name == _playerParam._mycharData._myCharData._name) _playerParam.DeadAction();
                  else
                  {
                      var target = _enemyParams.Where(x => x._mycharData != null
                      && chars.Equals(x._mycharData._myCharData)).FirstOrDefault();
                      if (target != null) target.DeadAction();
                  }
              });
        });
    }

    void EndTurnAction()
    {
        _uiActionQueue.Enqueue(() =>
        {
            AddDisplayText(_battlelogText);
            ChengeUIState(BattleUIState.DisplayText);
        });
    }
    void EndBattleAction(bool plDead)
    {
        _uiActionQueue.Enqueue(() =>
        {
            string log = "";
            if (plDead)
            {
                log += string.Format("コウたちは全滅した<w>\n");
                log += string.Format("目の前が真っ暗になった");
            }
            else
            {
                log += string.Format("コウは戦闘に勝利した！<w>\n");
                log += string.Format("経験値やお金を手に入れた！\n");
            }
            AddDisplayText(log);
            ChengeUIState(BattleUIState.DisplayText);
            _battleState = BattleState.Close;
        });
    }
    #endregion

    AbstractParamDisplay GetParamDisplayer(SavedDBData_char chars)
    {
        string charname = chars._name;
        if (charname == _playerParam._mycharData._myCharData._name)
        {
            return _playerParam;
        }
        else
        {
            var target = _enemyParams.Where(
                x => x._mycharData != null
                && charname == x._mycharData._myCharData._name).FirstOrDefault();

            return target;
        }
    }
}
