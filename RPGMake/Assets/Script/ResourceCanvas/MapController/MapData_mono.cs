using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData_mono : MonoBehaviour
{
    [SerializeField] Transform _firstPos;
    [SerializeField] Player.DIRECTION _firstDirection;

    public void SetPlayerPos()
    {
        Player.Instance.SetPosition(_firstPos.position, _firstDirection);
    }
}
