using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DBDInterface;

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

    [SerializeField, Space(10)] ChracterFieldDisplayer _charParamDisplyer;
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

    ICommandData temp_command;
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
    
    public void EndCommand(ICommandData command,string target,UIBase coalSelf)
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
        if (_displayText == null)_displayText = new List<string>();
        //すべての文字列を結合して分けなおす
        //_displayTextが使用されるまでの間にAddDisplayTextしたものは1つの塊として扱いたかった
        var fullText = string.Join("", _displayText)+text;
        var splited = fullText.Split('$');
        _displayText = splited.ToList();
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
        _charParamDisplyer.SetData(BattleController_mono.Instance.battle._charcterField);
    }
    public void SetUpBattleDelegate(BattleController battle)
    {
        battle._battleAction_encount = EncountAction;
        battle._battleAction_waitInput = () =>{_uiActionQueue.Enqueue(()=> ChengeUIState(BattleUIState.WaitInput));};
        battle._battleAction_command = CommandAction;
        battle._battleAction_item = ItemAction;
        battle._battleAction_attack = AttackAction;
        battle._battleAction_end = EndBattleAction;
        battle._battleAction_levelUp = LevelUpAction;
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
        });
    }
    //string _battlelogText = "";
    void CommandAction(BattleChar user, SkillCommandData skilldata)
    {
        AddDisplayText($"{user._displayName}の{skilldata._skillName}\n");
        _uiActionQueue.Enqueue(() =>
        {
            var target = _charParamDisplyer.GetParamDisplayer(user);
            target.SyncDisply();
            ChengeUIState(BattleUIState.DisplayText);
        });
    }
    void ItemAction(BattleChar user, ItemData itemdata)
    {
        AddDisplayText($"{user._displayName}は{itemdata._displayName}を使った\n");
    }

    void AttackAction(bool isCure,bool isDefeat,BattleChar target, int damage)
    {
        var log = "";
        if (isCure) log += $"{target._displayName}は{damage}回復<{target._displayName}0>\n";
        else log += $"{target._displayName}は{damage}のダメージ<{target._displayName}0>を受けた\n";
        if (isDefeat) log += $"{target._displayName}は倒れた<{target._displayName}1>\n";
        AddDisplayText(log);
        //表示の更新
        var targetDisp = _charParamDisplyer.GetParamDisplayer(target);
        if (targetDisp == null) return;
        _battleTextDisplayer.AddTextAction(target._displayName + "0", () => {
            targetDisp.SyncDisply();
            if (!isCure) targetDisp.DamageAction();
        });
        _battleTextDisplayer.AddTextAction(target._displayName + "1", () =>
        {
            targetDisp.DeadAction();
        });
    }
    void EndBattleAction(bool plDead,int money,int exp)
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
            log += string.Format($"{money}G手に入れた\n");
            log += string.Format($"経験値を{exp}手に入れた\n");
        }
        AddDisplayText(log);

        _uiActionQueue.Enqueue(() =>
        {
            ChengeUIState(BattleUIState.DisplayText);
            _battleState = BattleState.Close;
        });
    }

    void LevelUpAction(PlayerChar target,int up,List<SkillDBData> addSkillList)
    {
        if (up <= 0) return;

        var plName = target._displayName;
        var nowlevel = target._PlayerData._level;
        var beforelevel = nowlevel- up;

        var log = "";
        if (up == 1) log += $"{plName}はレベルが{nowlevel}に上がった<{plName}0>！\n";
        else if (up > 1)log += $"{plName}はレベルが{beforelevel}から{nowlevel}に上がった<{plName}0>！\n";
        foreach (var skill in addSkillList)
        {
            log += $"{plName}は{skill._Data._skillName}を習得した\n";
        }
        AddDisplayText(log);
        //表示の更新
        var targetDisp = _charParamDisplyer.GetParamDisplayer(target);
        if (targetDisp == null) return;
        _battleTextDisplayer.AddTextAction(target._displayName + "0", () => {
            target.SyncData_Data2This();
            targetDisp.SyncDisply();
        });
    }
    #endregion
    
}
