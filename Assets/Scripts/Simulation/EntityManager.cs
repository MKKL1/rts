using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Simulation
{

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
        public void SetGoal(uint[] netIds, Vector2Int goal)
        {
            GameMain gamemain = GameMain.instance;
            foreach (var id in netIds)
            {
                Entity entity = NetworkServer.spawned[id].transform.GetComponent<Entity>();
                //if(entityTransform.GetComponent<Entity>().ownerID == )
                //TODO easier method for placing objects on top of terrain
                Vector2 goalpos = gamemain.mainGrid.GetWorldPosition(goal.x, goal.y);
                entity.Move(new Vector3(goalpos.x, gamemain.mainTerrain.SampleHeight(new Vector3(goalpos.x, 0, goalpos.y)), goalpos.y));
            }
        }
    }
}