using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create SpriteDataBase", fileName = "SpriteDataBase")]
public class SpriteDataBase : ScriptableObject
{
    [SerializeField] List<SpriteDataSet> _spriteDataList;

    public Sprite GetSprite(string setName,int setIndex)
    {
        foreach(var data in _spriteDataList)
        {
            if (data.name == setName)
            {
                return data._spriteSet[setIndex];
            }
        }
        return null;
    }
}
