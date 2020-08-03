using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#region query
public abstract class UIQuery
{
    public bool _isCalled { get; private set; } = false;//このクエリが呼ばれたかどうか
    public bool _isSuccess { get; private set; } = false;//呼ばれた後処理の実行に成功したかどうか

    public bool _repeat { get; set; } = false;//成功するまで繰り返すかどうか

    // bool 成功したかどうか クエリが呼ばれたときによばれる
    Action<bool> _callback_endCall { get; set; }

    //実際のクエリ
    protected Action _mainAction;
    //処理を実行
    //UIControllerから呼ばれる
    public void Call(bool success, UIController uc)
    {
        if (success)
        {
            _mainAction.Invoke();
            EndAction(true);
        }
        else
        {
            if (_repeat) Register();
            else
            {
                EndAction(false);
            }
        }
    }

    void EndAction(bool success)
    {
        _isCalled = true;
        _isSuccess = success;
        _callback_endCall?.Invoke(success);
    }
    #region メソッドチェイン用
    public UIQuery AddEndAction(Action<bool> act)
    {
        _callback_endCall += act;
        return this;
    }
    public UIQuery Repeat()
    {
        _repeat = true;
        return this;
    }
    public UIQuery Register()
    {
        UIController.Instance.SendQuery(this);
        return this;
    }
    #endregion
}

public class AddUI : UIQuery
{
    public AddUI(UIBase next)
    {
        _mainAction = () =>
        {
            var uictrl = UIController.Instance;
            uictrl.AddUI_action(next, this);
        };
    }

}
public class CloseUI : UIQuery
{
    public CloseUI(UIBase next)
    {
        _mainAction = () =>
            {
                var uictrl = UIController.Instance;
                uictrl.CloseUI_action(next, this);
            };
    }
}
public class CloseToUI : UIQuery
{
    public CloseToUI(UIBase next)
    {
        _mainAction = () =>
            {
                var uictrl = UIController.Instance;
                uictrl.CloseToUI_action(next, this);
            };
    }
}

#endregion
public class UIController : SingletonMonoBehaviour<UIController>
{
    Stack<UIBase> _opnedUIStack = new Stack<UIBase>();//開いているuiを積んでいる
    int _TopSortOrder { get { return _opnedUIStack.Count; } }
    [SerializeField] UIBase _firstUI;

    WaitFlag _chengeInterbalFlag = new WaitFlag();
    Queue<UIQuery> _uiQuerys = new Queue<UIQuery>();//クエリをためておくqueue
    #region flash
    Dictionary<string, SavedDBData> _flashDBData_save = new Dictionary<string, SavedDBData>();
    Dictionary<string, AbstractDBData> _flashDBData_static = new Dictionary<string, AbstractDBData>();
    Dictionary<string, string> _flashData_st = new Dictionary<string, string>();
    #endregion
    //ここから設定。増えたら分離
    [SerializeField] float _chengeInterbal;//変更から次の変更可能までの待ち時間

    //uiの動作を開始できるかどうか
    bool _OperateEnable
    {
        get
        {
            if (_chengeInterbalFlag._waitNow) return false;
            else if (_OperateNow) return true;
            else if (GameContoller.Instance._AnyOperate) return false;
            else return true;
        }
    }
    //uiが動作中かどうか
    public bool _OperateNow
    {
        get
        {
            if (_opnedUIStack.Count == 0) return false;
            return _opnedUIStack.Peek()._IsOperateUI;
        }
    }
    //==============================================
    #region クエリの作成
    //処理の流れ　
    //クエリを作成 ココ
    //->クエリを登録　registerで実行
    //->クエリを呼び出し　lateUpdateで実行
    public static AddUI AddUI(UIBase next)
    {
        return new AddUI(next);
    }
    public static CloseUI CloseUI(UIBase target)
    {
        return new CloseUI(target);
    }
    public static CloseToUI CloseToUI(UIBase target)
    {
        return new CloseToUI(target);
    }
    #endregion
    public void Start()
    {
        _chengeInterbalFlag.SetWaitLength(_chengeInterbal);
        //AddUI(_firstUI,ignore:true);

        AddUI(_firstUI).Repeat().Register();
    }
    private void LateUpdate()
    {
        //クエリを順に解決していく
        bool havequery = _uiQuerys.Count != 0;
        if (havequery)
        {
            var query = _uiQuerys.Dequeue();
            bool success = _OperateEnable;
            query.Call(success, this);
            if (success) _chengeInterbalFlag.WaitStart(); ;
        }
    }
    #region UI操作
    public void SendQuery(UIQuery query)
    {
        _uiQuerys.Enqueue(query);
    }
    public void AddUI_action(UIBase next, UIQuery query)
    {
        if (_opnedUIStack.Count > 0)
        {
            var nowTop = _opnedUIStack.Peek();
            if (nowTop.Equals(next)) return;

            nowTop.SetUIState(UIBase.UIState.SLEEP);
        }

        _opnedUIStack.Push(next);
        next.SetUIState(UIBase.UIState.ACTIVE);
        next.SetSortOrder(_TopSortOrder);
    }

    public void CloseUI_action(UIBase target, UIQuery query)
    {
        var head = _opnedUIStack.Peek();
        while (head != target)
        {
            head = _opnedUIStack.Pop();
            head.SetUIState(UIBase.UIState.CLOSE);
            head = _opnedUIStack.Peek();
        }
        head = _opnedUIStack.Pop();
        head.SetUIState(UIBase.UIState.CLOSE);
        head = _opnedUIStack.Peek();
        head.SetUIState(UIBase.UIState.ACTIVE);
    }

    public void CloseToUI_action(UIBase target, UIQuery query)
    {
        var head = _opnedUIStack.Peek();
        while (head != target)
        {
            head = _opnedUIStack.Pop();
            head.SetUIState(UIBase.UIState.CLOSE);
            head = _opnedUIStack.Peek();
        }
        head.SetUIState(UIBase.UIState.ACTIVE);
    }
    #endregion
    #region flashData
    public void SetFlashData(string key, SavedDBData data)
    {
        _flashDBData_save.Add(key, data);
    }
    public void SetFlashData(string key, AbstractDBData data)
    {
        _flashDBData_static.Add(key, data);
    }
    public void SetFlashData(string key, string data)
    {
        _flashData_st.Add(key, data);
    }

    public AbstractDBData GetFlashData_static(string key)
    {
        if (_flashDBData_static.ContainsKey(key))
        {
            var data = _flashDBData_static[key];
            _flashDBData_static.Remove(key);
            return data;
        }
        else
        {
            return null;
        }
    }
    public SavedDBData GetFlashData_saved(string key)
    {
        if (_flashDBData_save.ContainsKey(key))
        {
            var data = _flashDBData_save[key];
            _flashDBData_save.Remove(key);
            return data;
        }
        else
        {
            return null;
        }
    }
    public string GetFlashData_string(string key)
    {
        if (_flashData_st.ContainsKey(key))
        {
            var data = _flashData_st[key];
            _flashData_st.Remove(key);
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
}
