using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFlagSetter : MonoBehaviour
{

    [System.Serializable]
    public class TermFlagSet
    {

        [SerializeField]public EventCodeScriptable flagTerm;
        [SerializeField]public string TargetflagName;
    }

    [SerializeField] bool _autoSetMode=true;
    [SerializeField] List<TermFlagSet> _flagSetList;


    private void Update()
    {
        if (!_autoSetMode) return;
        foreach(var flag in _flagSetList)
        {
            if (flag.flagTerm.CoalEnable())
            {
                SaveDataHolder.Instance.SetFlagNum(flag.TargetflagName, 1);
            }
            else
            {
                SaveDataHolder.Instance.SetFlagNum(flag.TargetflagName, 0);
            }
        }
    }
}
