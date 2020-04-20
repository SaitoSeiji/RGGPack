using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUIOperator : MonoBehaviour
{
    public enum OperateType
    {
        ADD,CLOSE,CLOSETO
    }
    [SerializeField] OperateType _operateType;
    [SerializeField] UIBase _nextUI;
    UIBase _myUIBase;

    

    protected abstract bool OperateTerm();

    private void Start()
    {
        _myUIBase = GetComponent<UIBase>();
        if (_myUIBase == null)
        {
            Debug.Log(string.Format("{0}'s UIOperator is not attached UIBase", gameObject.name));
        }
    }

    private void Update()
    {
        if (OperateTerm())
        {
            switch (_operateType)
            {
                case OperateType.ADD:
                    _myUIBase.AddUI(_nextUI);
                    break;
                case OperateType.CLOSE:
                    _myUIBase.CloseUI(_nextUI);
                    break;
                case OperateType.CLOSETO:
                    _myUIBase.CloseToUI(_nextUI);
                    break;
            }

        }
    }
}
