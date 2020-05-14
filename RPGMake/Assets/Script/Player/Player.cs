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
    public DIRECTION _NowDirection { get { return _nowDirection; } }
    public bool _moving { get; private set; } = false;
    [SerializeField] ColliderMessanger _bodyCollider;
    [SerializeField] ColliderMessanger _frontCollier;
    [SerializeField] SpriteRenderer _headIcon;
    Transform _plTr;
    Rigidbody2D rb;
    [SerializeField,Space(10)] PlayerSetting _setting;
    [SerializeField] Sprite checkSprite;

    bool _actEnable = true;
    ChengeFlag _gmCheckFlag = new ChengeFlag();

    bool _inited = false;

    public void Init()
    {
        _plTr = transform;
        rb = GetComponent<Rigidbody2D>();
        _frontCollier.AddTag("CheckEvent");
        _frontCollier.Init(transform);
        _bodyCollider.AddTag("HitEvent");
        _bodyCollider.Init(transform);
        SetPlayerMoveFlag();

        _inited = true;
    }
    void Update()
    {
        if (!_inited) return;
        MoveFlagUpdate();
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
        HitUpdate(_bodyCollider.CheckHitNow());
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

    void SetPlayerMoveFlag()
    {
        _gmCheckFlag.SetFlag(GameContoller.Instance._AnyOperate);
        _gmCheckFlag.SetAction(true, () => StopMove());
        _gmCheckFlag.SetAction(false, () => StartMove());
    }

    void MoveFlagUpdate()
    {

        _gmCheckFlag.CheckFlag(GameContoller.Instance._AnyOperate);
    }
    #endregion
    #region 移動
    void MoveUpdate(float tate,float yoko)
    {
        bool move = false;
        if (Mathf.Abs(tate) > _setting._inputIgnore)
        {
            SetVelocity(0,Mathf.Sign(tate) * _setting._moveSpeed * Time.fixedDeltaTime);
            move = true;
        }
        else if (Mathf.Abs(yoko) > _setting._inputIgnore)
        {
            SetVelocity(Mathf.Sign(yoko) * _setting._moveSpeed * Time.fixedDeltaTime,0);
            move = true;
        }
        else
        {
            SetVelocity(0, 0);
        }
        _moving = move;
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

    //毎フレームGetComponentしかねないので改善したい
    void HitUpdate(bool hit)
    {
        if (hit)
        {
            var hitEvent = _bodyCollider._nowHitCollider.GetComponent<EventDataMonoBehaviour>();
            if (hitEvent.CheckCoalEnable())
            {
                EventController.Instance.CoalEvent(hitEvent);
            }
        }
    }
}
