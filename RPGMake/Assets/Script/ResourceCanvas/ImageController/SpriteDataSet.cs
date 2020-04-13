using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyGame/Data/Create SpriteDataSet", fileName = "SpriteDataSet")]
public class SpriteDataSet : ScriptableObject
{
    [SerializeField] public List<Sprite> _spriteSet;


}