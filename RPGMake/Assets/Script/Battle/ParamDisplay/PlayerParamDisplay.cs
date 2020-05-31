using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerParamDisplay : AbstractParamDisplay
{
    public PlayerChar _mycharData { get; private set; }
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _spText;
    public void SetChar(PlayerChar data)
    {
        _mycharData = data;
    }
    protected override BattleChar GetChar()
    {
        return _mycharData;
    }
    protected override void DeadAction_child()
    {
        //赤にする　をしたいけどそのうち
    }

    public override void SyncDisply()
    {
        _hpText.text = GetHpText();
        _spText.text = GetSpText();
    }
    string GetHpText()
    {
        return string.Format("Hp:{0}/{1}", _mycharData._nowHp, _mycharData._maxHp);
    }
    string GetSpText()
    {
        return string.Format("Sp:{0}/{1}", _mycharData._nowSP, _mycharData._maxSP);
    }
}
