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

    public enum UIState_b
    {
        None,
        NoUI,//UIでやることがない状態
        Wait,//処理の完了待ち
        WaitInput,
        DisplayText,
        
    }
    [SerializeField,NonEditable]UIState_b _nowUIState;

    //キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] UIBase _baseUI;//バトルのUIの親
    [SerializeField] UIBase _commandUI;//コマンド入力を管理するUI
    [SerializeField] UIBase _textUI;//テキスト表示用のUI
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
    bool _temp_win;
    Queue<Action> _uiActionQueue=new Queue<Action>();
    

    private void OnDisable()
    {
        //_nextUIState = UIState_b.None;
        _nowUIState = UIState_b.None;
        _battleState = BattleState.None;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void Update()
    {
        bool exsistUIAction = _nowUIState != UIState_b.NoUI;
        if (!exsistUIAction)
        {
            if (_uiActionQueue.Count > 0)
            {
                _uiActionQueue.Dequeue().Invoke();
                exsistUIAction = _nowUIState != UIState_b.NoUI;
            }
        }


        if (exsistUIAction)
        {
            UIStateUpdate();
        }
        else
        {
            BattleStateUpdate(_battleState);
        }
        
    }
    //戦闘処理の進行
    void BattleStateUpdate(BattleState battleState)
    {
        switch (battleState)
        {
            case BattleState.Battle:
                {
                    BattleController_mono.Instance.Next();
                }
                break;
            case BattleState.Close:
                {
                    ChengeUIState(UIState_b.None);
                    _battleState = BattleState.None;
                }
                break;
        }
    }

    //戦闘UIの制御
    void UIStateUpdate()
    {
        switch (_nowUIState)
        {
            case UIState_b.WaitInput:
                if (_BaseUI._NowUIState == UIBase.UIState.ACTIVE)
                {
                    BattleController_mono.Instance.SetCharInput(temp_target, temp_command);
                    EndUIState(UIState_b.WaitInput);
                }
                break;
            case UIState_b.DisplayText:
                if (!_battleTextDisplayer._readNow)
                {
                    EndUIState(UIState_b.DisplayText);
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
    //UIの状態を変更する
    //入力は_nextUIStateに保持しワンクッション挟んでいる
    void ChengeUIState(UIState_b state)
    {
        _nowUIState = UIState_b.Wait;
        switch (state)
        {
            case UIState_b.DisplayText:
                UIController.AddUI(_textUI).Repeat().Register().AddEndAction(succsess => {
                    DisplayText(_displayText);
                    _nowUIState = UIState_b.DisplayText;
                });
                break;
            case UIState_b.WaitInput:
                UIController.AddUI(_commandUI).Repeat().Register().AddEndAction((suc) => _nowUIState = UIState_b.WaitInput);
                break;
            case UIState_b.None:
                bool win = _temp_win;
                LoadCanvas.Instance.StartBlack(auto: win);
                LoadCanvas.Instance._callback_blackend += () =>
                {
                    //_BaseUI.CloseUI(_BaseUI);
                    UIController.CloseUI(_BaseUI).Repeat().Register();
                    if (!win) SaveDataController.Instance.LoadAction();
                };
                LoadCanvas.Instance._callback_clearend +=()=>_nowUIState = UIState_b.None;
                break;
        }
    }
    void EndUIState(UIState_b state)
    {
        switch (state)
        {
            case UIState_b.DisplayText:
                _displayText = null;
                UIController.CloseToUI(_BaseUI).Repeat().Register().AddEndAction((suc)=>_nowUIState= UIState_b.NoUI);
                break;
            default:
                _nowUIState = UIState_b.NoUI;
                break;
        }
    }
    #endregion
    public void SetUpCharData()
    {
        _nowUIState = UIState_b.NoUI;
        _charParamDisplyer.SetData(BattleController_mono.Instance.battle._charcterField);
    }
    public void SetUpBattleDelegate(BattleController battle)
    {
        battle._battleAction_encount = EncountAction;
        battle._battleAction_waitInput = () => _uiActionQueue.Enqueue(() =>
        {
            //UIController.AddUI(_commandUI).Repeat().Register().AddEndAction((suc) => _nowUIState = UIState_b.WaitInput);
            ChengeUIState(UIState_b.WaitInput);
        });
        battle._battleAction_command = CommandAction;
        battle._battleAction_item = ItemAction;
        battle._battleAction_attack = AttackAction;
        battle._battleAction_end = EndBattleAction;
        battle._battleAction_levelUp = LevelUpAction;
    }

    public bool IsBattleNow()
    {
        return _nowUIState != UIState_b.None;
    }
    #region battleActionのデリゲート登録用関数
    void EncountAction()
    {
        _uiActionQueue.Enqueue(() =>
        {
            string log = "魔物が現れた";
            AddDisplayText(log);
            ChengeUIState(UIState_b.DisplayText);
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
            ChengeUIState(UIState_b.DisplayText);
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
        _temp_win = !plDead;
        _uiActionQueue.Enqueue(() =>
        {
            ChengeUIState(UIState_b.DisplayText);
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
