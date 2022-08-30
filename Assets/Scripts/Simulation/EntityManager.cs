using Assets.Scripts.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityManager
    {
        //Synchronized by spawning and despawning entities
        public List<Entity> visibleEntityList = new List<Entity>();

        public void addEntity(Entity entity)
        {
            visibleEntityList.Add(entity);
        }

        public void removeEntity(Entity entity)
        {
            visibleEntityList.Remove(entity);
        }
        public void removeEntity(uint netId)
        {
            visibleEntityList.First(x => x.netId == netId);
        }

        public List<Entity> GetEntitiesInBounds(Bounds bounds)
        {
            return visibleEntityList.FindAll(x => x.CollidesWith(bounds));
        }

        [Command]
        public void CmdSetEntityGoal(uint[] netIds, Vector2Int goal)
        {
            ServerScript.instance.entityMovement.SetEntitiesGoal(netIds, goal);
        }
    }
}