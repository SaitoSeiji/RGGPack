using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ButtonController : MonoBehaviour
{
    [SerializeField]private GameObject firstSelect;
    private CanvasGroup canvasGroup;

    [SerializeField] GameObject _nowSelect;

    WaitFlag _inputWaitFlag = new WaitFlag();
    public bool _InputEnable
    {
        get
        {
            return !_inputWaitFlag._waitNow;
        }
    }

    void OnEnable()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void SetButtonActive(bool flag)
    {
        canvasGroup.interactable = flag;
        if (flag)
        {
            EventSystem.current.SetSelectedGameObject(firstSelect);
        }
    }

    public void SetSelectButton(GameObject target)
    {
        if (canvasGroup.interactable)
        {
            EventSystem.current.SetSelectedGameObject(target);
        }
    }

    public void Update()
    {
        //クリックされたときにフォーカスが外れる問題の対策
        if (!canvasGroup.interactable) return;

        var temp = EventSystem.current.currentSelectedGameObject;
        if (temp == null)
        {
            EventSystem.current.SetSelectedGameObject(_nowSelect);
        }
        else
        {
            if (_nowSelect != temp) ButtonChengeAction();
            _nowSelect = temp;
        }
    }

    void ButtonChengeAction()
    {
        if (!_inputWaitFlag._endSetUp)
        {
            _inputWaitFlag.SetWaitLength(0.2f);
        }
        _inputWaitFlag.WaitStart();
    }
}
