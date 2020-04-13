using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteCanvas : SingletonMonoBehaviour<SpriteCanvas>
{
    [SerializeField] Image image_center;
    [SerializeField] Image image_back;
    [SerializeField] SpriteDataBase _dataBase;
    [SerializeField] Sprite clearImage;
    void SetImage(Image rend,Sprite sp)
    {
        rend.sprite = sp;
    }
    public void SetImageCenter(string spName, int spIndex)
    {
        SetImage(image_center, _dataBase.GetSprite(spName, spIndex));
    }

    public void SetImageBack(string spName, int spIndex)
    {
        SetImage(image_back, _dataBase.GetSprite(spName, spIndex));
    }
    
    void ResetImage(Image rend)
    {
        rend.sprite = clearImage;
    }

    public void ResetImage_Center()
    {
        ResetImage(image_center);
    }
    public void ResetImage_Back()
    {
        ResetImage(image_back);
    }
}
