using System.Collections;
using System.Collections.Generic;
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

        public List<Entity> GetEntitiesInBounds(Bounds bounds)
        {
            return visibleEntityList.FindAll(x => x.CollidesWith(bounds));
        }
    }
}