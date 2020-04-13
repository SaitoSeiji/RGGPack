using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Create MapDataScriptable", fileName = "MapData")]
public class MapDataScriptable : ScriptableObject
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
