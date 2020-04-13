using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCanvas : SingletonMonoBehaviour<LoadCanvas>
{
    [SerializeField] Animator anim;

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

    WaitAction _waitAction;

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
    public void StartLoad()
    {
        anim.SetBool("IsBlack", true);
        _waitAction.CoalWaitAction(() => _loadState = LOADSTATE.TOBLACK, 0.5f);
    }

    [ContextMenu("toClear")]
    public void EndLoad()
    {
        anim.SetBool("IsBlack", false);
        _waitAction.CoalWaitAction(() => _loadState = LOADSTATE.TOCLEAR, 0.5f);
    }

    void LoadStateUpdate()
    {
        var nowState = anim.GetCurrentAnimatorStateInfo(0);
        if (_LoadState == LOADSTATE.TOBLACK)
        {
            if (nowState.normalizedTime > 1)
            {
                _loadState = LOADSTATE.BLACK;
            }
        }
        else if (_LoadState == LOADSTATE.TOCLEAR)
        {
            if (nowState.normalizedTime > 1)
            {
                _loadState = LOADSTATE.CLEAR;
            }
        }

    }
}
