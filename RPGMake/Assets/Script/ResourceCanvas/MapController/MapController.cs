using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapController : SingletonMonoBehaviour<MapController>
{
    MapDataBase _mapData { get { return ResourceDB_mono.Instance._mapDB; } }
    MapData_mono _nowMapObject;
    LoadCanvas _loadCanvas;

    //string _beforeMapName;
    //string _nextMapName;
    public bool _mapChengeNow { get; private set; }

    [SerializeField, NonEditable] string _nowMapName;
    public string _NowMapName { get { return _nowMapName; } }

    int processCount;
    

    private void Start()
    {
        _loadCanvas = LoadCanvas.Instance;
    }

    private void Update()
    {
        //if (_mapChengeNow)
        //{
        //    switch (processCount)
        //    {
        //        case 0:
        //            _loadCanvas.StartBlack();
        //            processCount++;
        //            break;
        //        case 1:
        //            if (_loadCanvas.IsBlackNow)
        //            {
        //                ChengeMapObject(_nextMapName);
        //                _nowMapName = _nextMapName;
        //                _loadCanvas.StartClear();
        //                processCount++;
        //            }
        //            break;
        //        case 2:
        //            if (_loadCanvas.IsClearNow)
        //            {
        //                processCount = 0;
        //                _mapChengeNow = false;
        //            }
        //            break;
        //    }
        //}
    }

    void ChengeMapObject(string nextMapName,string beforeMapName,bool ignore=false)
    {
        var data = _mapData.mapDataList.Where(x => x._MapName == nextMapName).FirstOrDefault();
        if (data == null)
        {
            Debug.Log("MapController:mapName is not exist:"+nextMapName);
            return;
        }
        DestoryMap();
        _nowMapObject = CreatMapObject(data).GetComponent<MapData_mono>();
        if(!ignore)_nowMapObject.SetPlayerPos(beforeMapName);//プレイヤーの位置を指定
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

    public void ChengeMap(string mapName,bool ignoreMapPos = false,bool fromBlack=false)
    {
        //_beforeMapName = _nextMapName;
        //_nextMapName = mapName;
        _mapChengeNow = true;

        if (fromBlack)
        {
            ChengeMapObject(mapName, _nowMapName, ignoreMapPos);
            _nowMapName = mapName;
            _loadCanvas.StartClear();
        }
        else
        {
            _loadCanvas.StartBlack();
            _loadCanvas._callback_blackend += () =>
            {
                ChengeMapObject(mapName, _nowMapName, ignoreMapPos);
                _nowMapName = mapName;
            };
        }
        _loadCanvas._callback_clearend += () =>
        {
            _mapChengeNow = false;
        };
        
    }
    

    [SerializeField] string testMapName;
    [ContextMenu("chengeMap")]
    public void ChengeMap_test()
    {
        ChengeMap(testMapName);
    }
}
