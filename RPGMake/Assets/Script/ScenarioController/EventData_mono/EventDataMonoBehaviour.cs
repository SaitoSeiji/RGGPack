using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventDataMonoBehaviour : MonoBehaviour
{
    [SerializeField]EventCodeScriptable _eventData;

    private void Start()
    {
    }

    public void EventAction()
    {
        EventCodeReadController.Instance.SetEventData(_eventData);
        EventCodeReadController.Instance.StartEvent();
    }

    public bool CheckCoalEnable()
    {
        return _eventData.CoalEnable();
    }

    //調べられるかどうかの更新処理
    bool FindEnable()
    {
        return CheckCoalEnable();
    }
}
