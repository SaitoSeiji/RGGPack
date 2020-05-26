using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonUnitDisplayer : MonoBehaviour
{
    [SerializeField] Button _myButton;
    [SerializeField] Text _myText;
    [SerializeField] Text _additionalText;
    [SerializeField] Image _myImage;
    ButtonData.ButtonType _buttonType;
    
    public void SetDisplayData(string mainText,string addtionalText,Sprite _imageSprite,ButtonData.ButtonType buttonType)
    {
        _myText.text = mainText;
        if (_myImage != null)
        {
            _myImage.sprite = _imageSprite;
        }

        if (_additionalText != null)
        {
            _additionalText.text = addtionalText;
        }
        _buttonType = buttonType;
        SetColor(buttonType);
    }
    #region buttoncolor
    void SetButtonColors_unselectable()
    {
        var colors = _myButton.colors;
        colors.normalColor = _myButton.colors.disabledColor;
        colors.selectedColor = (colors.disabledColor + colors.selectedColor) / 2;
        _myButton.colors = colors;
    }

    //全体攻撃など複数選択するとき用
    void SetButtonColors_semiselect()
    {
        var colors = _myButton.colors;
        colors.normalColor = (colors.normalColor*0.3f + colors.selectedColor*0.7f);
        _myButton.colors = colors;
    }

    void SetColor(ButtonData.ButtonType buttontype)
    {
        switch (buttontype)
        {
            case ButtonData.ButtonType.Selectable:
                break;
            case ButtonData.ButtonType.Unselectable:
                SetButtonColors_unselectable();
                break;
            case ButtonData.ButtonType.Selected:
                SetButtonColors_semiselect();
                break;
        }
    }
    #endregion
    public void SetOnClick(UnityEvent ue)
    {
        if (_buttonType == ButtonData.ButtonType.Unselectable) return;
        _myButton.onClick.AddListener(()=>ue.Invoke());
    }
}
