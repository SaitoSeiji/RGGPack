using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "MyGame/Create SpriteDataBase", fileName = "SpriteDataBase")]
public class SpriteDataBase : ScriptableObject
{
    [System.Serializable]
    public class SpriteData
    {
        public string setName;
        public List<Sprite> _dataList = new List<Sprite>();
    }

    [SerializeField] List<SpriteData> _spriteDataList;

    public Sprite GetSprite(string setName,int setIndex)
    {
        var dataSet=_spriteDataList.Where(x => x.setName == setName).FirstOrDefault();
        try
        {
            return dataSet._dataList[setIndex];
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("SpriteDataBase:DataSetName is not exsist");
            return null;
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.Log("SpriteDataBase:index is not exsist");
            return null;
        }
        
    }
}
