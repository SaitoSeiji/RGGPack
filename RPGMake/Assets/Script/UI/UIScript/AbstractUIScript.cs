using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//各UIの個別処理を簡単に書くためのスクリプト
//スクリプトという言葉を「簡単な実装をするためのコード」という意味で使っている
public abstract class AbstractUIScript : MonoBehaviour,IChengeUIState
{
    UIBase _myUIBase;
    protected UIBase _MyUIBase
    {
        get
        {
            if (_myUIBase == null) _myUIBase = GetComponent<UIBase>();
            return _myUIBase;
        }
    }


    protected abstract List<ButtonData> CreateMyButtonData();
    protected virtual void ChengedUIStateAction(UIBase.UIState chenged)
    {
        switch (chenged)
        {
            case UIBase.UIState.ACTIVE:
                var list = CreateMyButtonData();
                if (list != null)
                {
                    _MyUIBase.ResetButtonData();
                    _MyUIBase.AddButtonData(list);
                    _MyUIBase.SyncButtonToText();
                }
                break;
            case UIBase.UIState.SLEEP:
                break;
            case UIBase.UIState.CLOSE:
                break;
        }
    }

    void IChengeUIState.RecieveChenge(UIBase.UIState changed)
    {
        ChengedUIStateAction(changed);
    }
}
