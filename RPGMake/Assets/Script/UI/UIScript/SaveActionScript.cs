using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveActionScript : AbstractUIScript
{
    protected override List<ButtonData> CreateMyButtonData()
    {
        return null;
    }

    public void Onclick_saveAction()
    {
        SaveDataController.Instance.SaveAction();
    }
}
