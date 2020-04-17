using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EventCodeReadControlInspector : MonoBehaviour
{
    [SerializeField] EventCodeReadController ctrl;
    [SerializeField] EventCodeScriptable scr;

    [ContextMenu("CoalEvent")]
    public void CoalEvent()
    {
        ctrl.SetEventData(scr);
        ctrl.StartEvent();
    }
}
