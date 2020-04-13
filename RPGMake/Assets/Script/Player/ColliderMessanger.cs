using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColliderMessanger : MonoBehaviour
{
    [SerializeField] BoxCollider2D _hitCollider;
    [SerializeField] List<string> _colliderTagList;
    [SerializeField] bool anyHit;

    Transform _parentTr;
    Vector2 _hitPoint;
    Vector2 _hitSize;

    bool _start = false;
    public Collider2D _nowHitCollider { get; private set; }
    
    public void Init(Transform parent)
    {
        _parentTr = parent;
        _hitSize = _hitCollider.size;
        _start = true;
    }

    public void SetHitPoint(float x,float y)
    {
        _hitPoint.x = x;
        _hitPoint.y = y;
        _hitCollider.transform.position = _parentTr.position+(Vector3)_hitPoint;
    }

    public void AddTag(string tag)
    {
        _colliderTagList.Add(tag);
    }

    public bool CheckHitNow()
    {
        if (!_start) return false;
        var hits= Physics2D.OverlapBoxAll(_hitPoint+(Vector2)_parentTr.position,_hitSize,0);
        foreach(var hit in hits)
        {
            if (anyHit) return true;
            if (_colliderTagList.Contains(hit.tag))
            {
                _nowHitCollider = hit;
                return true;
            }
        }
        return false;
    }
}
