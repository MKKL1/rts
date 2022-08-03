using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;
    public Bounds bounds;

    private Collider _collider;
    private void Start()
    {
        _collider = GetComponent<Collider>();
        bounds = _collider.bounds;
        GameMain.instance.addEntity(this);
    }

    public bool CollidesWith(Bounds _bounds)
    {
        return (bounds.min.x <= _bounds.max.x) && (bounds.max.x >= _bounds.min.x) &&
            (bounds.min.z <= _bounds.max.z) && (bounds.max.z >= _bounds.min.z);
    }
}
