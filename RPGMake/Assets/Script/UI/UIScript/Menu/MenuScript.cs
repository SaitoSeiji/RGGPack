using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : AbstractUIScript_onclick
{
    [SerializeField] ChracterFieldDisplayer _playerField;

    public override void OnclickAction()
    {
        throw new System.NotImplementedException();
    }

    protected override void ChengeState_toActive()
    {
        base.ChengeState_toActive();
        var cf = new CharcterField(CharcterField.GetParty(),new List<EnemyChar>());
        _playerField.SetData(cf);
    }
}
