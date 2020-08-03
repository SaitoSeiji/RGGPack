using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIBase : MonoBehaviour
{
    public enum UIState
    {
        ACTIVE,SLEEP,CLOSE
    }
    [SerializeField] UIState _nowUIstate = UIState.CLOSE;
    public UIState _NowUIState { get { return _nowUIstate; } }
    [SerializeField] bool _isOperateUI = true;
    public bool _IsOperateUI { get { return _isOperateUI; } }
    #region キャッシュ
    Canvas _myCanvas;
    public RectTransform _myPanel { get; private set; }
    public ButtonController _myButtonController { get; private set; }
    #endregion
    private void Awake()
    {
        Init();
    }

    void Init()
    {
        if (_myCanvas != null) return;
        _myCanvas = GetComponent<Canvas>();
        _myPanel = transform.GetChild(0).GetComponent<RectTransform>();
        _myButtonController = _myPanel.GetComponent<ButtonController>();
    }
    void ButtonUpdate()
    {
        Init();
        if (_myButtonController == null) return;
        switch (_nowUIstate)
        {
            case UIState.ACTIVE:
                _myButtonController.SetButtonActive(true);
                break;
            case UIState.SLEEP:
            case UIState.CLOSE:
                _myButtonController.SetButtonActive(false);
                break;
        }
    }
    //ここからpublic
    #region セット系
    public void SetSortOrder(int i)
    {
        Init();
        _myCanvas.sortingOrder = i;
    }

    public void SetUIState(UIState target)
    {
        _nowUIstate = target;
        switch (_nowUIstate)
        {
            case UIState.CLOSE:
                //アクティブがfalseになるとメッセージが飛ばせないので処理を1フレーム送らせている
                //不格好なので治せるなら直す
                WaitAction.Instance.CoalWaitAction_frame(()=> gameObject.SetActive(false),1);
                break;
            case UIState.SLEEP:
                gameObject.SetActive(true);
                break;
            case UIState.ACTIVE:
                gameObject.SetActive(true);
                break;
        }
        ButtonUpdate();
        SendMessage_chengeState(_nowUIstate);
    }
    #endregion
    #region ボタンで設定する操作
    public void AddUI(UIBase next)
    {
        if (_nowUIstate != UIState.ACTIVE) return;
        UIController.AddUI(next).Register();
        //UIController.Instance.AddUI(next);//処理準問題が再発生したらしたのにする
        //WaitAction.Instance.CoalWaitAction_frame(()=>UIController.Instance.AddUI(next),1);
    }

    public void CloseUI(UIBase target)
    {
        if (_nowUIstate != UIState.ACTIVE) return;
        UIController.CloseUI(target).Register();
        //UIController.Instance.CloseUI(target);
        //WaitAction.Instance.CoalWaitAction_frame(() => UIController.Instance.CloseUI(target), 1);
    }

    public void CloseToUI(UIBase next)
    {
        if (_nowUIstate != UIState.ACTIVE) return;
        UIController.CloseToUI(next).Register();
        //UIController.Instance.CloseToUI(next);
        //WaitAction.Instance.CoalWaitAction_frame(() => UIController.Instance.CloseToUI(next), 1);
    }
    #endregion

    void SendMessage_chengeState(UIState changed)
    {
        ExecuteEvents.Execute<IChengeUIState>(
          target: gameObject,
          eventData: null,
          functor: (reciever, eventData) => reciever.RecieveChenge(changed)
        );
    }
}
