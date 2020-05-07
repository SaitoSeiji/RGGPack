using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharParamDisplay : MonoBehaviour
{
    BattleChar _mycharData;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] Image _charImage;
    [SerializeField] bool _deadRemove;//死亡したら表示を消す
    [SerializeField] Sprite clear;

    Animator anim;
    int _beforeHp;

    public void SetChar(BattleChar data)
    {
        _mycharData = data;
        _name.text = data._myCharData._name;
        _charImage.sprite = data._myCharData._charImage;
        if (_charImage.sprite == null) _charImage.sprite = clear;
        SyncData();
        gameObject.SetActive(true);
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        _beforeHp = data._myCharData._Hp;
    }


    public void SyncData()
    {
        if (_mycharData == null) return;
        if (_hpText != null)
        {
            _hpText.text = GetHpText();
        }
    }
    public void DamageAction()
    {
        if (_mycharData == null) return;
        if (_beforeHp > _mycharData._nowHp)
        {
            anim.SetTrigger("damage");
        }
        _beforeHp = _mycharData._nowHp;
    }

    public void DeadAction()
    {
        if (_mycharData == null) return;
        if (_mycharData._nowHp <= 0 && _deadRemove)
        {
            gameObject.SetActive(false);
            _mycharData = null;
        }
    }

    string GetHpText()
    {
        return string.Format("{0}/{1}",_mycharData._nowHp,_mycharData._myCharData._Hp);
    }
    
}
