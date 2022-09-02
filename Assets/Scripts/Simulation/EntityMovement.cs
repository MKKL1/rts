using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    public struct MovingEntity
    {
        public Entity entity;
        public MovementPath path;
    }

    public class EntityMovement
    {
        public int tickPerSecond = 30;
        public List<MovingEntity> movingEntities = new List<MovingEntity>();

        private GameMain gameMain;
        private PathFinding pathFinding;
        public EntityMovement()
        {
            gameMain = GameMain.instance;
            pathFinding = new PathFinding(gameMain.mainGrid);
        }
        public void SetEntityGoal(Entity entity, Vector2 goal)
        {
            movingEntities.RemoveAll(e => e.entity == entity);
            //TODO remove temporary pathfinding to center of grid cell
            MovementPath enpath = pathFinding.GetPath(entity.transform.position, goal);
            //TODO indicator to player that path was not found
            if(enpath == null)
            {
                Debug.Log("Path not found");
                return;
            }
            movingEntities.Add(new MovingEntity()
            {
                entity = entity,
                path = enpath
            });
        }

        public void SetEntitiesGoal(uint[] netIds, Vector2 goal)
        {
            foreach (var id in netIds)
            {
                Entity entity = NetworkServer.spawned[id].transform.GetComponent<Entity>();
                SetEntityGoal(entity, goal);
            }
        }

        public void SetEntitiesGoal(Entity[] entities, Vector2 goal)
        {
            foreach(Entity entity in entities)
            {
                SetEntityGoal(entity, goal);
            }
        }

        //private float nextTickTime = 0;
        //private float lastTickTime = 0;
        //public void Update()
        //{
        //    if(Time.time > nextTickTime)
        //    {
        //        lastTickTime = nextTickTime;
        //        nextTickTime = Time.time + (1f / (float)tickPerSecond);
        //        OnMovementTick();
        //    }
        //}

        public void FixedUpdate()
        {
            OnMovementTick(Time.fixedDeltaTime);
        }

        public virtual void OnMovementTick(float tickDeltaTime)
        {
            for(int i = movingEntities.Count - 1; i >= 0; i--)
            {
                MovingEntity movingEntity = movingEntities.ElementAt(i);
                Transform entityTransform = movingEntity.entity.transform;
                if(movingEntity.path.ShouldChangeDirection(new Vector2(entityTransform.position.x, entityTransform.position.z)))
                {
                    if(movingEntity.path.ShouldGetNext())
                    {
                        //Rotate to look at point
                        Vector2 pathPoint = movingEntity.path.GetNextPoint();
                        Vector3 pathPoint3 = new Vector3(pathPoint.x, 0, pathPoint.y);
                        entityTransform.rotation = Quaternion.LookRotation(pathPoint3 - entityTransform.position, Vector3.up);
                    }
                    else
                    {
                        //Finished moving to goal
                        movingEntities.RemoveAt(i);
                        continue;
                    }
                }
                entityTransform.position += entityTransform.forward * tickDeltaTime * movingEntity.entity.movementSpeed;
            }
            
        }
    }
}