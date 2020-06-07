using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FullParamDisplay : AbstractParamDisplay
{
    public PlayerChar _mycharData { get; private set; }
    [SerializeField] TextMeshProUGUI _paramText;
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
    }

    public override void SyncDisply()
    {
        _paramText.text = $"Lv{_mycharData._PlayerData._level}\n" +
                          $"HP:{GetHpText()}\n" +
                          $"SP:{GetSpText()}\n" +
                          $"攻撃力:{_mycharData._PlayerData._attack}\n" +
                          $"防御力:{_mycharData._PlayerData._guard}\n" +
                          $"次のレベルまで:{_mycharData._PlayerData.CalcNextLevelExp()}";
    }
    string GetHpText()
    {
        return string.Format("{0}/{1}", _mycharData._nowHp, _mycharData._maxHp);
    }
    string GetSpText()
    {
        return string.Format("{0}/{1}", _mycharData._nowSP, _mycharData._maxSP);
    }

}
