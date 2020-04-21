﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingletonMonoBehaviour<UIController>
{
    Stack<UIBase> _opnedUIList = new Stack<UIBase>();
    int _TopSortOrder { get { return _opnedUIList.Count; } }
    [SerializeField] UIBase _firstUI;

    WaitFlag _chengeInterbalFlag = new WaitFlag();
    Dictionary<string, DBData> _flashDBData=new Dictionary<string, DBData>();
    //ここから設定。増えたら分離
    [SerializeField] float _chengeInterbal;
    
    bool _OperateEnablbe
    {
        get
        {
            return !_chengeInterbalFlag._waitNow && !EventController.Instance.GetReadNow();
        }
    }

    public bool _PlayerMoveEnable
    {
        get
        {
            if (_opnedUIList.Count == 0) return true;
            return _opnedUIList.Peek()._PermitPlayerMove;
        }
    }
    //===============================================
    public void Start()
    {
        _chengeInterbalFlag.SetWaitLength(_chengeInterbal);
        AddUI(_firstUI,ignore:true);
    }

    /// <summary>
    /// 追加でnextを開く
    /// </summary>
    public void AddUI(UIBase next,bool ignore=false)
    {
        if (!_OperateEnablbe&&!ignore) return;
        if (_opnedUIList.Count > 0)
        {
            var nowTop = _opnedUIList.Peek();
            nowTop.SetUIState(UIBase.UIState.SLEEP);
        }

        _opnedUIList.Push(next);
        next.SetUIState(UIBase.UIState.ACTIVE);
        next.SetSortOrder(_TopSortOrder);

        _chengeInterbalFlag.WaitStart();
    }
    #region UI操作
    /// <summary>
    /// targetまで閉じる（targetは閉じる）
    /// </summary>
    /// <param name="target"></param>
    public void CloseUI(UIBase target, bool ignore=false)
    {
        if (!_OperateEnablbe && !ignore) return;
        var head = _opnedUIList.Peek();
        while (head != target)
        {
            head = _opnedUIList.Pop();
            head.SetUIState(UIBase.UIState.CLOSE);
            head = _opnedUIList.Peek();
        }
        head = _opnedUIList.Pop();
        head.SetUIState(UIBase.UIState.CLOSE);
        head = _opnedUIList.Peek();
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
        var head = _opnedUIList.Peek();
        while (head != target)
        {
            head = _opnedUIList.Pop();
            head.SetUIState(UIBase.UIState.CLOSE);
            head = _opnedUIList.Peek();
        }
        head.SetUIState(UIBase.UIState.ACTIVE);


        _chengeInterbalFlag.WaitStart();
    }
    #endregion

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
}