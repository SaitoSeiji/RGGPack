using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDB_mono : SingletonMonoBehaviour<ResourceDB_mono>
{
    public AudioDataBase _audioDB;
    public SpriteDataBase _imageDB;
    public MapDataBase _mapDB;

    [Space(10)]public ShopDBData _nowShopData;
    [SerializeField] SkillDBData _attackData;
    public SkillDBData _AttackData
    {
        get
        {
            if (_attackData == null) Debug.LogError($"{gameObject.name}-ResourceDB_monoの_attackDataにデータをセットしてください");
            return _attackData;
        }
    }
}
