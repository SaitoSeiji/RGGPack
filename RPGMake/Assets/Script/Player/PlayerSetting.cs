using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSetting", menuName = "Setting/PlayerSetting", order = 0)]
public class PlayerSetting : ScriptableObject
{
    [SerializeField] public float _moveSpeed;
    [SerializeField, Range(0, 1)]public float _inputIgnore;
    [SerializeField] public Vector2 flontPos;
}
