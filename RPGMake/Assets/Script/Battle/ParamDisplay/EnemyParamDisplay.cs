using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyParamDisplay : AbstractParamDisplay
{
    public BattleChar _mycharData { get; private set; }
    LayoutGroup _layout;

    public void SetChar(BattleChar data)
    {
        _mycharData = data;
    }
    protected override BattleChar GetChar()
    {
        return _mycharData;
    }
    protected override void DeadAction_child()
    {
        _active = false;
        gameObject.SetActive(false);
    }
    public override void SyncDisply()
    {

    }

    public override void Activate()
    {
        base.Activate();
        _layout = GetComponent<VerticalLayoutGroup>();
        UpdateLayout(_layout);
    }

    //layoutが反映されないときに更新をする処理
    //非アクティブの時に要素を変更し、activeにすると発生？
    static void UpdateLayout(LayoutGroup layout)
    {
        //layoutがアクティブになったタイミングだと機能しないので1frame待っている
        //本当は、必要なら1フレーム待つ　をしたい
        WaitAction.Instance.CoalWaitAction_frame(() =>
        {
            layout.CalculateLayoutInputVertical();
            layout.SetLayoutVertical();
        }, 1);
    }
}
