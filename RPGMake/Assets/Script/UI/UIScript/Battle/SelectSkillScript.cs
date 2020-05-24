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
        var useAbleSkillList = BattleController_mono.Instance.GetSkillList();
        var resultList = new List<ButtonData>();
        foreach (var data in useAbleSkillList)
        {
            var add = new ButtonData(data._Data._skillName, CreateClickEvent(data), CreateCursorEvent(data));
            resultList.Add(add);
            var btr =new Battle_useResource(data._Data._useResourceType,data._Data._useNum,BattleController_mono.Instance.battle._player);
            add.SetIsActive(btr.IsUseable());
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
        //var ct = BattleController_mono.Instance.battle.GetTargetPool(data._Data);
        //対象選択をする場合
        //if (ct.IsInputSelect())
        if (Battle_targetDicide.IsInputSelect(data._Data._target))
            {
            UIController.Instance.SetFlashData("command", data);
            _MyUIBase.AddUI(_targetSelectUI);
        }
        else//しない場合
        {
            BattleUIController.Instance.EndCommand(data._Data, null, _MyUIBase);
        }
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
