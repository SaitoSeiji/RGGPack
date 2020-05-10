using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetInspectorSkillScript : AbstractUIScript_onclick
{
    public void OnclickAction_skill(SkillDBData _onclickSkill)
    {
        var saveddb = SaveDataController.Instance.GetDB_static<SkillDB>()._dataList;
        var data = saveddb.Where(x => x._Data._serchId == _onclickSkill._Data._serchId).First();
        UIController.Instance.SetFlashData("command", data._Data);
    }

    public override void OnclickAction()
    {
        throw new System.NotImplementedException();
    }
}
