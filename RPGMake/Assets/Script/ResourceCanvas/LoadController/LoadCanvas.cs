using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public void StartBlack()
    {
        if (!IsClearNow) return;
        DOTween.To(()=>_loadAlpha,num=> _loadAlpha=num,1,_fadeSpeed);
        _loadState = LOADSTATE.TOBLACK;
    }

    [ContextMenu("toClear")]
    public void StartClear()
    {
        if (!IsBlackNow) return;
        DOTween.To(() => _loadAlpha, num => _loadAlpha = num, 0, _fadeSpeed);
        _loadState = LOADSTATE.TOCLEAR;
    }

    void LoadStateUpdate()
    {
        if (!IsBlackNow && !IsClearNow)
        {
            var cl = _fadePanel.color;
            cl.a = _loadAlpha;
            _fadePanel.color = cl;
            if (_loadState == LOADSTATE.TOBLACK&&cl.a==1.0f)
            {
                _loadState = LOADSTATE.BLACK;
            }else if (_loadState == LOADSTATE.TOCLEAR && cl.a == 0.0f)
            {
                _loadState = LOADSTATE.CLEAR;
            }
        }
    }
}
