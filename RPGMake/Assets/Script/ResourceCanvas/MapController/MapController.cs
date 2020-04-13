using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapController : SingletonMonoBehaviour<MapController>
{
    [SerializeField] MapDataBase _mapData;
    MapData_mono _nowMapObject;
    LoadCanvas _loadCanvas;

    string _beforeMapName;
    string _nextMapName;
    public bool _mapChengeNow { get; private set; }

    int processCount;
    

    private void Start()
    {
        _loadCanvas = LoadCanvas.Instance;
    }

    private void Update()
    {
        if (_mapChengeNow)
        {
            switch (processCount)
            {
                case 0:
                    _loadCanvas.StartBlack();
                    processCount++;
                    break;
                case 1:
                    if (_loadCanvas.IsBlackNow)
                    {
                        ChengeMapObject(_nextMapName);
                        _loadCanvas.StartClear();
                        processCount++;
                    }
                    break;
                case 2:
                    if (_loadCanvas.IsClearNow)
                    {
                        processCount = 0;
                        _mapChengeNow = false;
                    }
                    break;
            }
        }
    }

    void ChengeMapObject(string mapName)
    {
        var data = _mapData.mapDataList.Where(x => x._MapName == mapName).FirstOrDefault();
        if (data == null)
        {
            Debug.Log("MapController:mapName is not exist:"+mapName);
            return;
        }
        DestoryMap();
        _nowMapObject = CreatMapObject(data).GetComponent<MapData_mono>();
        _nowMapObject.SetPlayerPos(_beforeMapName);
    }

    GameObject CreatMapObject(MapDataBase.MapData data)
    {
        var obj =Instantiate(data._MapObject, Vector2.zero, Quaternion.identity);
        return obj;
    }

    void DestoryMap()
    {
        if (_nowMapObject == null) return;
        Destroy(_nowMapObject.gameObject);
    }

    public void ChengeMap(string mapName)
    {
        _beforeMapName = _nextMapName;
        _nextMapName = mapName;
        _mapChengeNow = true;
    }


    [SerializeField] string testMapName;
    [ContextMenu("chengeMap")]
    public void ChengeMap_test()
    {
        ChengeMap(testMapName);
    }
}
