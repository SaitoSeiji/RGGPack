using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public abstract class AbstractParamDisplay : MonoBehaviour
{
    Animator anim;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] Image _charImage;


    int _beforeHp;//ダメージを食らったかどうかを自動で判定するためのもの
    protected bool _active;
    BattleChar _chardata;

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    void Init()
    {
        anim = GetComponent<Animator>();
    }
    
    public virtual void Activate()
    {
        Init();
        _chardata = GetChar();
        gameObject.SetActive(true);
        _name.text = _chardata._myCharData._name;
        _charImage.sprite = _chardata._myCharData._charImage;
        SyncDisply();
        _active = true;
        _beforeHp = _chardata._nowHp;//_beforeHp初期設定
    }
    
    protected abstract BattleChar GetChar();
    protected abstract void DeadAction_child();

    public abstract void SyncDisply();
    public void DamageAction()
    {
        if (!_active) return;
        bool damaged = IsDamaged();
        if (damaged)
        {
            anim.SetTrigger("damage");
        }
    }
    public void DeadAction()
    {
        if (!_active) return;
        if (_chardata._nowHp <= 0)
        {
            DeadAction_child();
        }
    }

    bool IsDamaged()
    {
        bool damaged = _beforeHp > _chardata._nowHp;
        _beforeHp = _chardata._nowHp;
        return damaged;
    }
}
