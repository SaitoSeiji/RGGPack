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
    SpriteDataBase _dataBase { get { return ResourceDB_mono.Instance._imageDB; } }
    [SerializeField] Sprite clearImage;
    #region setImage
    public void SetImage(string spName,int spIndex,ImageCode.ImagePos pos)
    {
        var sp = _dataBase.GetSprite(spName, spIndex);
        switch (pos)
        {
            case ImageCode.ImagePos.BACK:
                SetImage(image_back, sp);
                break;
            case ImageCode.ImagePos.CENTER:
                SetImage(image_center, sp);
                break;
            case ImageCode.ImagePos.LEFT:
                SetImage(image_left, sp);
                break;
            case ImageCode.ImagePos.RIGHT:
                SetImage(image_right, sp);
                break;
        }
    }

    void SetImage(Image rend,Sprite sp)
    {
        rend.sprite = sp;
    }
    #endregion
    #region resetImage
    public void ResetImage(ImageCode.ImagePos pos)
    {
        switch (pos)
        {
            case ImageCode.ImagePos.BACK:
                ResetImage(image_back);
                break;
            case ImageCode.ImagePos.CENTER:
                ResetImage(image_center);
                break;
            case ImageCode.ImagePos.LEFT:
                ResetImage(image_left);
                break;
            case ImageCode.ImagePos.RIGHT:
                ResetImage(image_right);
                break;
        }
    }

    void ResetImage(Image rend)
    {
        rend.sprite = clearImage;
    }
    #endregion
    
    public void SetDirection(ImageCode.ImagePos pos,ImageCode.ImageDirection dir)
    {
        switch (pos)
        {
            case ImageCode.ImagePos.BACK:
                SetDirection(image_back,GetDirectionScale(dir));
                break;
            case ImageCode.ImagePos.CENTER:
                SetDirection(image_center, GetDirectionScale(dir));
                break;
            case ImageCode.ImagePos.LEFT:
                SetDirection(image_left, GetDirectionScale(dir));
                break;
            case ImageCode.ImagePos.RIGHT:
                SetDirection(image_right, GetDirectionScale(dir)*-1);
                break;
        }
    }

    void SetDirection(Image image,float scalex)
    {
        var scale = image.rectTransform.localScale;
        scale.x = scalex;
        image.rectTransform.localScale = scale;
    }

    float GetDirectionScale(ImageCode.ImageDirection dir)
    {
        switch (dir)
        {
            case ImageCode.ImageDirection.Origin:
                return 1;
            case ImageCode.ImageDirection.Reverse:
                return -1;
        }
        return 0;
    }
}
