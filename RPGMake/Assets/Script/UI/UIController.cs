using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingletonMonoBehaviour<UIController>
{
    Stack<UIBase> _opnedUIStack = new Stack<UIBase>();
    int _TopSortOrder { get { return _opnedUIStack.Count; } }
    [SerializeField] UIBase _firstUI;

    WaitFlag _chengeInterbalFlag = new WaitFlag();
    Dictionary<string, DBData> _flashDBData=new Dictionary<string, DBData>();
    //ここから設定。増えたら分離
    [SerializeField] float _chengeInterbal;
    
    bool _OperateEnablbe
    {
        get
        {
            if (_chengeInterbalFlag._waitNow) return false;
            else if (_OperateNow) return true;
            else if (GameContoller.Instance._AnyOperate) return false;
            else return true;
        }
    }

    public bool _OperateNow
    {
        get
        {
            if (_opnedUIStack.Count == 0) return false;
            return _opnedUIStack.Peek()._IsOperateUI;
        }
    }
    //===============================================
    public void Start()
    {
        _chengeInterbalFlag.SetWaitLength(_chengeInterbal);
        AddUI(_firstUI,ignore:true);
    }

    #region UI操作
    /// <summary>
    /// 追加でnextを開く
    /// </summary>
    public void AddUI(UIBase next,bool ignore=false)
    {
        Debug.Log("add:"+next.name);
        if (!_OperateEnablbe&&!ignore) return;
        if (_opnedUIStack.Count > 0)
        {
            var nowTop = _opnedUIStack.Peek();
            if (nowTop.Equals(next)) return;

            nowTop.SetUIState(UIBase.UIState.SLEEP);
        }

        _opnedUIStack.Push(next);
        next.SetUIState(UIBase.UIState.ACTIVE);
        next.SetSortOrder(_TopSortOrder);

        _chengeInterbalFlag.WaitStart();

        Debug.Log("add:succsess" + next.name);
    }
    /// <summary>
    /// targetまで閉じる（targetは閉じる）
    /// </summary>
    /// <param name="target"></param>
    public void CloseUI(UIBase target, bool ignore=false)
    {
        if (!_OperateEnablbe && !ignore) return;
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


        _chengeInterbalFlag.WaitStart();
    }

    /// <summary>
    /// targeまで閉じる(targetは閉じない)
    /// </summary>
    /// <param name="target"></param>
    public void CloseToUI(UIBase target, bool ignore=false)
    {
        if (!_OperateEnablbe && !ignore) return;
        var head = _opnedUIStack.Peek();
        while (head != target)
        {
            head = _opnedUIStack.Pop();
            head.SetUIState(UIBase.UIState.CLOSE);
            head = _opnedUIStack.Peek();
        }
        head.SetUIState(UIBase.UIState.ACTIVE);


        _chengeInterbalFlag.WaitStart();
    }
    #endregion
    #region flashData
    public void SetFlashData(string key,DBData data)
    {
        _flashDBData.Add(key, data);
    }

    public DBData GetFlashData(string key)
    {
        if (_flashDBData.ContainsKey(key))
        {
            var data = _flashDBData[key];
            _flashDBData.Remove(key);
            return data;
        }
        else
        {
            return null;
        }
    }
    #endregion
}
