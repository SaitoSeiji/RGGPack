using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDB_mono : SingletonMonoBehaviour<ResourceDB_mono>
{
    public AudioDataBase _audioDB;
    public SpriteDataBase _imageDB;
    public MapDataBase _mapDB;

    public ShopDBData _nowShopData;
    public SkillDBData _attackData;
}
