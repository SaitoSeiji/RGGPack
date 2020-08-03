using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LoadCanvas : SingletonMonoBehaviour<LoadCanvas>
{

    public enum LOADSTATE
    {
        BLACK,
        CLEAR,
        TOBLACK,
        TOCLEAR
    }
    [SerializeField] LOADSTATE _loadState;
    public LOADSTATE _LoadState { get { return _loadState; } }
    public bool IsBlackNow { get { return _LoadState == LOADSTATE.BLACK; } }
    public bool IsClearNow { get { return _LoadState == LOADSTATE.CLEAR; } }
    [SerializeField] Image _fadePanel;
    WaitAction _waitAction;
    float _loadAlpha;
    float _fadeSpeed = 1.0f;

    public Action _callback_blackend;
    public Action _callback_clearend;

    private void Start()
    {
        _loadState = LOADSTATE.CLEAR;
        _waitAction = WaitAction.Instance;
    }

    private void Update()
    {
        LoadStateUpdate();
    }

    [ContextMenu("toBlack")]
    public void StartBlack(bool auto=true)
    {
        if (!IsClearNow) return;
        var sec=DOTween.To(()=>_loadAlpha,num=> _loadAlpha=num,1,_fadeSpeed);
        sec.onComplete = () =>
        {
            _loadState = LOADSTATE.BLACK;
            _callback_blackend?.Invoke();
              _callback_blackend = null;
            if (auto) StartClear();
          };
        _loadState = LOADSTATE.TOBLACK;
    }

    [ContextMenu("toClear")]
    public void StartClear()
    {
        if (!IsBlackNow) return;
        var sec=DOTween.To(() => _loadAlpha, num => _loadAlpha = num, 0, _fadeSpeed);
        sec.onComplete = () =>
        {
            _loadState = LOADSTATE.CLEAR;
            _callback_clearend?.Invoke();
            _callback_clearend = null;
        };
        _loadState = LOADSTATE.TOCLEAR;
    }

    void LoadStateUpdate()
    {
        if (!IsBlackNow && !IsClearNow)
        {
            var cl = _fadePanel.color;
            cl.a = _loadAlpha;
            _fadePanel.color = cl;
            //if (_loadState == LOADSTATE.TOBLACK&&cl.a==1.0f)
            //{
            //}else if (_loadState == LOADSTATE.TOCLEAR && cl.a == 0.0f)
            //{
            //    _loadState = LOADSTATE.CLEAR;
            //}
        }
    }
}
