using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataBases/DataBase/Create MapDataBase", fileName = "MapDataBase")]
public class MapDataBase : ScriptableObject
{
    [System.Serializable]
    public class MapData
    {
        [SerializeField] string _mapName;
        public string _MapName{get{ return _mapName; } }

        [SerializeField] GameObject _mapObject;
        public GameObject _MapObject { get { return _mapObject; } }
    }

    public List<MapData> mapDataList = new List<MapData>();
}
