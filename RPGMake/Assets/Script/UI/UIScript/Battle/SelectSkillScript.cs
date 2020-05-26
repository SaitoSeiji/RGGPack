using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using TMPro;

public class SelectSkillScript : AbstractUIScript_button
{
    [SerializeField] UIBase _targetSelectUI;

    [SerializeField, Space(10)] GameObject _textPannel;
    [SerializeField] TextMeshProUGUI setumeiText;

    protected override void ChengeState_toClose()
    {
        base.ChengeState_toClose();
        _textPannel.SetActive(false);
        setumeiText.text = "";
    }
    
    protected override List<ButtonData> CreateMyButtonData()
    {
        //var useAbleSkillList = BattleController_mono.Instance.GetSkillList();
        var pl = BattleController_mono.Instance.battle._charcterField._playerList[0];
        var useAbleSkillList = pl._myCharData._mySkillList;
        var resultList = new List<ButtonData>();
        foreach (var data in useAbleSkillList)
        {
            var btr = new Battle_useResource(data._Data._useResourceType, data._Data._useNum, pl);
            var btType = (btr.IsUseable()) ? ButtonData.ButtonType.Selectable : ButtonData.ButtonType.Unselectable;
            var add = new ButtonData(data._Data._skillName, CreateClickEvent(data), 
                                     CreateCursorEvent(data),
                                     btType);
            resultList.Add(add);
        }
        return resultList;
    }

    private UnityEvent CreateClickEvent(SkillDBData data)
    {
        UnityEvent ue = new UnityEvent();
        ue.AddListener(()=> {
            ClickNextUIEvent(data);
        });
        return ue;
    }

    void ClickNextUIEvent(SkillDBData data)
    {

        UIController.Instance.SetFlashData("command", data);
        _MyUIBase.AddUI(_targetSelectUI);
    }

    private Action CreateCursorEvent(SkillDBData data)
    {
        Action ue = () =>
        {
            _textPannel.SetActive(true);
            var information = BattleCommandDataFormatter.Format(data._Data,data._Data._skillName);
            setumeiText.text =information;
        };
        return ue;
    }
    
}
