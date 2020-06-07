using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetInspectorSkillScript : AbstractUIScript_onclick
{
    public void OnclickAction_skill(SkillDBData _onclickSkill)
    {
        var saveddb = SaveDataController.Instance.GetDB_static<SkillDB>()._dataList;
        var data = saveddb.Where(x => x.name == _onclickSkill.name).First();
        UIController.Instance.SetFlashData("command", data);
    }

    public override void OnclickAction()
    {
        var data=ResourceDB_mono.Instance._attackData;
        UIController.Instance.SetFlashData("command", data);
    }
}
