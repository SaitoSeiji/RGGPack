using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class StatusScript : AbstractUIScript_button
{
    [SerializeField] FullParamDisplay _paramDisplayer;

    protected override List<ButtonData> CreateMyButtonData()
    {
        var result = new List<ButtonData>();
        var db = SaveDataController.Instance.GetDB_var<PlayerDB, SavedDBData_player>();
        foreach(var pl in db)
        {
            Action cursorAction = () =>
            {
                _paramDisplayer.SetChar(new PlayerChar(pl));
                _paramDisplayer.Activate();
            };
            UnityEvent clickEvent = new UnityEvent();
            var add =new ButtonData(pl._Name,clickEvent,cursorAction, ButtonData.ButtonType.Selectable);
            result.Add(add);
        }

        return result;
    }
}
