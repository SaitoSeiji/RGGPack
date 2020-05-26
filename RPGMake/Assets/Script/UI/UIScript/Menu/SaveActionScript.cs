using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveActionScript : AbstractUIScript_onclick
{
    public override void OnclickAction()
    {
        SaveDataController.Instance.SaveAction();
    }
}
