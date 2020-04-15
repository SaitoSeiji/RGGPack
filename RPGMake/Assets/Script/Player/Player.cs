using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    public enum DIRECTION
    {
        NONE,UP,DOWN,LEFT,RIGHT
    }
    [SerializeField] DIRECTION _nowDirection;
    [SerializeField] ColliderMessanger _frontCollier;
    [SerializeField] SpriteRenderer _headIcon;
    Transform _plTr;
    Rigidbody2D rb;
    [SerializeField,Space(10)] PlayerSetting _setting;
    [SerializeField] Sprite checkSprite;

    bool _actEnable = true;
    ChengeFlag _chengeFlag = new ChengeFlag();

    bool _inited = false;

    public void Init()
    {
        _plTr = transform;
        rb = GetComponent<Rigidbody2D>();
        _frontCollier.AddTag("Check");
        _frontCollier.Init(transform);

        _chengeFlag.SetFlag(EventCodeReadController._getIsReadNow);
        _chengeFlag.SetAction(true, () => StopMove());
        _chengeFlag.SetAction(false, () => StartMove());

        _inited = true;
    }
    void Update()
    {
        if (!_inited) return;
        _chengeFlag.CheckFlag(EventCodeReadController._getIsReadNow);
        if (!_actEnable)
        {
            return;
        }
        float tate = Input.GetAxisRaw("Vertical");
        float yoko = Input.GetAxisRaw("Horizontal");
        bool submit = Input.GetKeyDown(KeyCode.Z);

        MoveUpdate(tate,yoko);
        _nowDirection = GetDirection(rb.velocity);
        DirectionUpdate(_nowDirection);
        CheckUpdate(_frontCollier.CheckHitNow(),submit);
    }
    #region 有効無効
    void StopMove()
    {
        MoveUpdate(0, 0);
        _headIcon.sprite = null;
        _actEnable = false;
    }

    void StartMove()
    {
        WaitAction.Instance.CoalWaitAction(() => _actEnable = true, 0.1f);
    }
    #endregion
    #region 移動
    void MoveUpdate(float tate,float yoko)
    {

        if (Mathf.Abs(tate) > _setting._inputIgnore)
        {
            SetVelocity(0,Mathf.Sign(tate) * _setting._moveSpeed * Time.fixedDeltaTime);
        }
        else if (Mathf.Abs(yoko) > _setting._inputIgnore)
        {
            SetVelocity(Mathf.Sign(yoko) * _setting._moveSpeed * Time.fixedDeltaTime,0);
        }
        else
        {
            SetVelocity(0, 0);
        }
    }

    void SetVelocity(float x,float y)
    {
        var vel = rb.velocity;
        vel.x = x;
        vel.y = y;
        rb.velocity = vel;
    }

    public void SetPosition(Vector2 pos,DIRECTION direct)
    {
        _plTr.position = pos;
        _nowDirection = direct;
        DirectionUpdate(_nowDirection);
    }
    #endregion

    #region Direction
    DIRECTION GetDirection(Vector2 vel)
    {
        float tate = vel.y;
        float yoko = vel.x;

        if (Mathf.Abs(tate) > _setting._inputIgnore)
        {
            if (tate > 0) return DIRECTION.UP;
            else return DIRECTION.DOWN;
        }
        else if (Mathf.Abs(yoko) > _setting._inputIgnore)
        {
            if (yoko > 0) return DIRECTION.LEFT;
            else return DIRECTION.RIGHT;
        }
        return _nowDirection;
    }

    void DirectionUpdate(DIRECTION direct)
    {
        switch (direct)
        {
            case DIRECTION.DOWN:
                _frontCollier.SetHitPoint(0,-_setting.flontPos.y);
                break;
            case DIRECTION.UP:
                _frontCollier.SetHitPoint(0, _setting.flontPos.y);
                break;
            case DIRECTION.LEFT:
                _frontCollier.SetHitPoint(_setting.flontPos.x,0);
                break;
            case DIRECTION.RIGHT:
                _frontCollier.SetHitPoint(-_setting.flontPos.x, 0);
                break;
        }
    }
    #endregion
    #region Check
    void CheckUpdate(bool check,bool submit)
    {
        if (check)
        {
            var checkEvent = _frontCollier._nowHitCollider.GetComponent<EventDataMonoBehaviour>();
            if (checkEvent.CheckCoalEnable())
            {
                _headIcon.sprite = checkSprite;
                if (submit)
                {
                    EventController.Instance.CoalEvent(checkEvent);
                }
            }
            else
            {
                _headIcon.sprite = null;
            }
        }
        else
        {
            _headIcon.sprite = null;
        }
    }
    #endregion
}
