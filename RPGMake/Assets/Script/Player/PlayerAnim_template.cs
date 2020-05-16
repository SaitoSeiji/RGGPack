using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim_template : MonoBehaviour
{
    Animator anim;

    Player.DIRECTION _plDirection;
    Player _player;
    bool _moveNow;
    #region monoBehaviour
    private void Start()
    {
        _player = GetComponent<Player>();
        anim = GetComponent<Animator>();
        _moveNow = false;
    }

    private void Update()
    {
        var nowDirection = _player._NowDirection;
        SwichDirection(_player._NowDirection);
        _plDirection = nowDirection;
        var nowMoving = _player._moving;
        if (_moveNow != nowMoving)
        {
            if (nowMoving) MoveStart();
            else MoveStop();
        }
        _moveNow = nowMoving;
        if (_moveNow) Moving();
    }
    #endregion
    //方向が変更になった時に呼ばれる
    void SwichDirection(Player.DIRECTION dir)
    {
        switch (dir)
        {
            case Player.DIRECTION.UP:
                break;
            case Player.DIRECTION.DOWN:
                break;
            case Player.DIRECTION.LEFT:
                break;
            case Player.DIRECTION.RIGHT:
                break;
        }
    }

    //移動を開始したときに呼ばれる
    void MoveStart()
    {

    }

    //移動を止めた時に呼ばれる
    void MoveStop()
    {

    }
    //移動をしているときに呼ばれる
    void Moving()
    {

    }
}
