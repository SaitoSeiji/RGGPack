using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingletonMonoBehaviour<UIController>
{
    Stack<UIBase> _opnedUIStack = new Stack<UIBase>();
    int _TopSortOrder { get { return _opnedUIStack.Count; } }
    [SerializeField] UIBase _firstUI;

    WaitFlag _chengeInterbalFlag = new WaitFlag();
    Dictionary<string, SavedDBData> _flashDBData_save=new Dictionary<string, SavedDBData>();
    Dictionary<string, AbstractDBData> _flashDBData_static=new Dictionary<string, AbstractDBData>();
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
    public void SetFlashData(string key, SavedDBData data)
    {
        _flashDBData_save.Add(key, data);
    }
    public void SetFlashData(string key, AbstractDBData data)
    {
        _flashDBData_static.Add(key, data);
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
    #endregion
}
