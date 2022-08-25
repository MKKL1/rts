using Assets.Scripts.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public string entityName;
    public Bounds bounds;
    public PlayerIdentificator ownerID = PlayerIdentificator.serverID;

    private Collider _collider;
    private void Start()
    {
        _collider = GetComponent<Collider>();
        bounds = _collider.bounds;
        GameMain.instance.entityManager.addEntity(this);
    }

    public bool CollidesWith(Bounds _bounds)
    {
        return (bounds.min.x <= _bounds.max.x) && (bounds.max.x >= _bounds.min.x) &&
            (bounds.min.z <= _bounds.max.z) && (bounds.max.z >= _bounds.min.z);
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
        bounds = _collider.bounds;
    }
}
