using Assets.Scripts.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    public string entityName;
    public Bounds bounds {get { return _collider.bounds; }}
    public PlayerIdentificator ownerID = PlayerIdentificator.serverID;
    public float movementSpeed = 20f;

    private Collider _collider;
    private void Start()
    {
        _collider = GetComponent<Collider>();
        GameMain.instance.entityManager.addEntity(this);
    }

    public bool CollidesWith(Bounds _bounds)
    {
        Bounds thisbounds = bounds;
        return (thisbounds.min.x <= _bounds.max.x) && (thisbounds.max.x >= _bounds.min.x) &&
            (bounds.min.z <= _bounds.max.z) && (thisbounds.max.z >= _bounds.min.z);
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }
}
