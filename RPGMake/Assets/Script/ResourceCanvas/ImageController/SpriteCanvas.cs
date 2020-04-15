using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCanvas : SingletonMonoBehaviour<SpriteCanvas>
{
    [SerializeField] Image image_center;
    [SerializeField] Image image_left;
    [SerializeField] Image image_right;
    [SerializeField] Image image_back;
    [SerializeField] SpriteDataBase _dataBase;
    [SerializeField] Sprite clearImage;
    #region setImage
    void SetImage(Image rend,Sprite sp)
    {
        rend.sprite = sp;
    }
    public void SetImageCenter(string spName, int spIndex)
    {
        SetImage(image_center, _dataBase.GetSprite(spName, spIndex));
    }
    public void SetImageLeft(string spName, int spIndex)
    {
        SetImage(image_left, _dataBase.GetSprite(spName, spIndex));
    }
    public void SetImageRight(string spName, int spIndex)
    {
        SetImage(image_back, _dataBase.GetSprite(spName, spIndex));
    }

    public void SetImageBack(string spName, int spIndex)
    {
        SetImage(image_back, _dataBase.GetSprite(spName, spIndex));
    }
    #endregion
    #region resetImage
    void ResetImage(Image rend)
    {
        rend.sprite = clearImage;
    }

    public void ResetImage_Center()
    {
        ResetImage(image_center);
    }
    public void ResetImage_Left()
    {
        ResetImage(image_left);
    }
    public void ResetImage_Right()
    {
        ResetImage(image_right);
    }
    public void ResetImage_Back()
    {
        ResetImage(image_back);
    }
    #endregion
}
