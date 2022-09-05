﻿using Mirror;
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

    public class EntityMovement : MonoBehaviour
    {
        public int tickPerSecond = 30;
        public List<MovingEntity> movingEntities = new List<MovingEntity>();

        private GameMain gameMain;
        private PathFinding pathFinding;
        private Terrain terrain;
        public void OnTerrainGenerated()
        {
            gameMain = GameMain.instance;
            terrain = gameMain.mainTerrain;
            pathFinding = new PathFinding(gameMain.mainGrid);
        }
        public void SetEntityGoal(Entity entity, Vector2 goal)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            movingEntities.RemoveAll(e => e.entity == entity);
            //TODO remove temporary pathfinding to center of grid cell
            MovementPath enpath = pathFinding.GetPath(entity.transform.position.GetWithoutY(), goal);
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

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Debug.Log(elapsedMs);
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

        private void FixedUpdate()
        {
            OnMovementTick(Time.fixedDeltaTime);
        }

        private void OnDrawGizmos()
        {
            foreach(var movingEntity in movingEntities)
            {
                Queue<Vector2> copiedPath = new Queue<Vector2>(movingEntity.path.points);
                Vector2 lastPos = movingEntity.entity.transform.position.GetWithoutY();

                while(copiedPath.Count > 0)
                {
                    Debug.Log(lastPos.GetWithY(50));
                    Gizmos.DrawLine(lastPos.GetWithY(50), copiedPath.Dequeue().GetWithY(50));
                }
            }
        }

        public virtual void OnMovementTick(float tickDeltaTime)
        {
            for(int i = movingEntities.Count - 1; i >= 0; i--)
            {
                MovingEntity movingEntity = movingEntities.ElementAt(i);
                Transform entityTransform = movingEntity.entity.transform;
                if(movingEntity.path.ShouldChangeDirection(entityTransform.position.GetWithoutY()))
                {
                    if(movingEntity.path.ShouldGetNext())
                    {
                        //Rotate to look at point
                        Vector2 pathPoint = movingEntity.path.GetNextPoint();
                        Vector3 pathPoint3 = new Vector3(pathPoint.x, 0, pathPoint.y);
                        entityTransform.rotation = Quaternion.LookRotation(pathPoint3 - entityTransform.position, Vector3.up);
                        entityTransform.eulerAngles = new Vector3(0, entityTransform.eulerAngles.y, 0);
                    }
                    else
                    {
                        //Finished moving to goal
                        movingEntities.RemoveAt(i);
                        continue;
                    }
                }
                Vector3 newPos = entityTransform.position;
                newPos += entityTransform.forward * tickDeltaTime * movingEntity.entity.movementSpeed;
                newPos.y = terrain.SampleHeight(entityTransform.position);
                entityTransform.position = newPos;
            }
            
        }
    }
}