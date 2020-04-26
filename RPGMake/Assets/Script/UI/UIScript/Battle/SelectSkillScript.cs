using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class SelectSkillScript : AbstractUIScript_button
{
    [SerializeField] UIBase nextUI;

    protected override List<ButtonData> CreateMyButtonData()
    {
        var saveddb = SaveDataController.Instance.GetDB_static<SkillDB>().GetDataList().Select(x=>x as SkillDBData);
        var dataList = BattleController_mono.Instance.GetSkillList();
        var resultList = new List<ButtonData>();
        foreach (var data in dataList)
        {
            var dbData = saveddb.Where(x => x._Data._serchId == data._Data._serchId).First();
            resultList.Add(new ButtonData(data._SKill._skillName, CreateClickEvent(dbData._Data)));
        }
        return resultList;
    }

    private UnityEvent CreateClickEvent(DBData data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=>UIController.Instance.SetFlashData("command",data));
        ue.AddListener(() => _MyUIBase.AddUI(nextUI));
        return ue;
    }
}
