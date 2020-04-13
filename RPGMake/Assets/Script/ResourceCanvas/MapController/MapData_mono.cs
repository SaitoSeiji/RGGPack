using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class MapChengeData
{
    [SerializeField]public string _beforeMapName;
    [SerializeField]public Transform _movePos;
    [SerializeField]public Player.DIRECTION _direction;

}

public class MapData_mono : MonoBehaviour
{
    [SerializeField]List<MapChengeData> _chengeList = new List<MapChengeData>();

    public void SetPlayerPos(string beforeMapName)
    {
        var target = _chengeList.Where(x => x._beforeMapName == beforeMapName).FirstOrDefault();
        if (target == null) target = _chengeList[0];
        Player.Instance.SetPosition(target._movePos.position, target._direction);
    }
}
