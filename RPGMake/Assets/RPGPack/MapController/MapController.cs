using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : SingletonMonoBehaviour<MapController>
{
    [SerializeField] MapChenger _mapChenger;
    LoadCanvas _loadCanvas;

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
                    _loadCanvas.StartLoad();
                    processCount++;
                    break;
                case 1:
                    if (_loadCanvas.IsBlackNow)
                    {
                        _mapChenger.ChengeMap(_nextMapName);
                        _loadCanvas.EndLoad();
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

    public void ChengeMap(string mapName)
    {
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
