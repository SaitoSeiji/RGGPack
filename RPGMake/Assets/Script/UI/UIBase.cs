using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class ButtonData
{
    public string _buttonText;
    public UnityEvent _onClickAction=new UnityEvent();

    public ButtonData(string _text, UnityEvent _onclick)
    {
        _buttonText = _text;
        _onClickAction = _onclick;
    }
    public ButtonData(string _text)
    {
        _buttonText = _text;
    }
}

public class UIBase : MonoBehaviour
{
    public enum UIState
    {
        ACTIVE,SLEEP,CLOSE
    }
    [SerializeField] UIState _nowUIstate = UIState.CLOSE;
    public UIState _NowUIState { get { return _nowUIstate; } }
    [SerializeField] bool _permitPlayerMove = false;
    public bool _PermitPlayerMove { get { return _permitPlayerMove; } }
    #region キャッシュ
    Canvas _myCanvas;
    RectTransform _myPanel;
    ButtonController _myButtonController;
    #endregion

    [SerializeField] Button _buttonPrefab;
    List<ButtonData> _buttonDataList=new List<ButtonData>();
    List<GameObject> _addButtonList = new List<GameObject>();
    [SerializeField] int _buttonDisplayRange;
    [SerializeField]int _buttonStartIndex;
    [SerializeField]int _nowSelectButtonIndex;
    

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        ButtonIndexUpdate();
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
                _myButtonController.SetButtonActive(false);
                break;
            case UIState.CLOSE:
                _myButtonController.SetButtonActive(false);
                ResetButtonData();
                SyncButtonToText();
                _nowSelectButtonIndex = 0;
                _buttonStartIndex = 0;
                break;
        }
    }
    #region buttonOperation
    public void AddButtonData(ButtonData data)
    {
        _buttonDataList.Add(data);
    }
    public void AddButtonData(List<ButtonData> data)
    {
        _buttonDataList.AddRange(data);
    }
    public void ResetButtonData()
    {
        _buttonDataList = new List<ButtonData>();
    }

    public void SyncButtonToText()
    {
        ResetButton();
        for(int i = 0; i < _buttonDisplayRange; i++)
        {
            if (_buttonDataList.Count <= i+_buttonStartIndex) break;
            var target = _buttonDataList[i + _buttonStartIndex];
            var bt=AddButton(target._buttonText);
            bt.onClick.AddListener(()=>target._onClickAction.Invoke());
        }
        if (_addButtonList.Count > 0)
        {
            if (_nowSelectButtonIndex < _addButtonList.Count)
            {
                _myButtonController.SetSelectButton(_addButtonList[_nowSelectButtonIndex].gameObject);
            }
            else
            {
                _myButtonController.SetSelectButton(_addButtonList[0].gameObject);
            }
        }
    }
    
    Button AddButton(string text)
    {
        var add = Instantiate(_buttonPrefab, _myPanel);
        var addText = add.GetComponentInChildren<Text>();
        addText.text = text;
        _addButtonList.Add(add.gameObject);
        return add;
    }
    
    public void ResetButton()
    {
        foreach(var bt in _addButtonList)
        {
            Destroy(bt.gameObject);
        }
        _addButtonList = new List<GameObject>();
    }

    int GetCurrentButtonIndex()
    {
        var select= EventSystem.current.currentSelectedGameObject;
        if (_addButtonList.Contains(select))
        {
            return _addButtonList.IndexOf(select);
        }
        return _nowSelectButtonIndex;
    }


    void ButtonIndexUpdate()
    {
        if (_addButtonList.Count > 0 && _myButtonController._InputEnable)
        {
            float tate = Input.GetAxisRaw("Vertical");
            if (_nowSelectButtonIndex + 1 == _buttonDisplayRange)
            {
                int downMax = _buttonDataList.Count - _buttonDisplayRange;
                if (tate < 0 && _buttonStartIndex < downMax)
                {
                    _buttonStartIndex++;
                    SyncButtonToText();
                }
            }
            else if (_nowSelectButtonIndex == 0)
            {
                if (tate > 0 && _buttonStartIndex > 0)
                {
                    _buttonStartIndex--;
                    SyncButtonToText();
                }
            }
            _nowSelectButtonIndex = GetCurrentButtonIndex();
        }
    }
    #endregion
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
                gameObject.SetActive(false);
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
        UIController.Instance.AddUI(next);
    }

    public void CloseUI(UIBase target)
    {
        if (_nowUIstate != UIState.ACTIVE) return;
        UIController.Instance.CloseUI(target);
    }

    public void CloseToUI(UIBase next)
    {
        if (_nowUIstate != UIState.ACTIVE) return;
        UIController.Instance.CloseToUI(next);
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
