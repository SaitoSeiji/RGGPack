using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnim_template : MonoBehaviour
{
    Animator anim;

    Player.DIRECTION _beforeDirection;
    Player _player;
    bool _beforeMove;

    #region callback
    public Action<Player.DIRECTION> _callback_swichDirection { get; set; }
    public Action _callback_stopMove { get; set; }
    public Action _callback_startMove { get; set; }
    public Action _callback_moving { get; set; }
    #endregion 
    #region monoBehaviour
    private void Start()
    {
        _player = GetComponent<Player>();
        anim = GetComponent<Animator>();
        _beforeMove = false;
    }

    private void Update()
    {
        //var nowDirection = _player._NowDirection;
        //if (nowDirection != _plDirection) SwichDirection(_player._NowDirection);
        //_plDirection = nowDirection;
        //var nowMoving = _player._moving;
        //if (_moveNow != nowMoving)
        //{
        //    if (nowMoving) MoveStart();
        //    else MoveStop();
        //}
        //_moveNow = nowMoving;
        //if (_moveNow) Moving()
        var nowDirection = _player._NowDirection;
        if (nowDirection != _beforeDirection) _callback_swichDirection?.Invoke( nowDirection);
        _beforeDirection = nowDirection;

        var nowMoving = _player._moving;
        if (_beforeMove != nowMoving)
        {
            if (nowMoving) _callback_startMove?.Invoke();
            else _callback_startMove?.Invoke();
        }
        _beforeMove = nowMoving;
        if (_beforeMove) _callback_moving?.Invoke();
    }
    #endregion
    ////方向が変更になった時に呼ばれる
    //void SwichDirection(Player.DIRECTION dir)
    //{
    //    switch (dir)
    //    {
    //        case Player.DIRECTION.UP:
    //            break;
    //        case Player.DIRECTION.DOWN:
    //            break;
    //        case Player.DIRECTION.LEFT:
    //            break;
    //        case Player.DIRECTION.RIGHT:
    //            break;
    //    }
    //}

    ////移動を開始したときに呼ばれる
    //void MoveStart()
    //{

    //}

    ////移動を止めた時に呼ばれる
    //void MoveStop()
    //{

    //}
    ////移動をしているときに呼ばれる
    //void Moving()
    //{

    //}
}
