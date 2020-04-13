using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//map変更のための関数を所持
[System.Serializable]
public class MapChenger
{
    [SerializeField] MapDataScriptable _mapData;

    GameObject _nowMapObject;
    public void ChengeMap(string mapName)
    {
        try
        {
            var data = _mapData.mapDataList.Where(x => x._MapName == mapName).First();

            DestoryMap();
            _nowMapObject = CreatMapObject(data);
            SetPlayerPos(_nowMapObject.GetComponent<MapData_mono>());
        }
        catch(System.InvalidOperationException)
        {
            Debug.Log("mapname is wrong :input mapname is "+mapName);
        }
    }

    GameObject CreatMapObject(MapDataScriptable.MapData data)
    {
        var obj = MonoBehaviour.Instantiate(data._MapObject, Vector2.zero, Quaternion.identity);
        return obj;
    }

    void DestoryMap()
    {
        if (_nowMapObject == null) return;
        MonoBehaviour.Destroy(_nowMapObject);
    }
    
    void SetPlayerPos(MapData_mono mapData)
    {
        Debug.Log("set pl pos : 未実装");
    }
}
