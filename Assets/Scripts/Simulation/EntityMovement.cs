using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    public class EntityMovement
    {
        public int tickPerSecond = 30;
        public List<Entity> movingEntities = new List<Entity>();

        private GameMain gameMain;
        private PathFinding pathFinding;
        public EntityMovement()
        {
            gameMain = GameMain.instance;
            pathFinding = new PathFinding(gameMain.mainGrid);
        }
        public void SetEntityGoal(Entity entity, Vector2Int goal)
        {
            //TODO easier method for placing objects on top of terrain
            Vector2 goalpos = gameMain.mainGrid.GetWorldPosition(goal.x, goal.y) + gameMain.mainGrid.worldCellSize * 0.5f;
            entity.Move(Utils.GetOnTopOfTerrain(goalpos, gameMain.mainTerrain));
        }

        public void SetEntitiesGoal(uint[] netIds, Vector2Int goal)
        {
            foreach (var id in netIds)
            {
                Entity entity = NetworkServer.spawned[id].transform.GetComponent<Entity>();
                SetEntityGoal(entity, goal);
            }
        }

        public void SetEntitiesGoal(Entity[] entities, Vector2Int goal)
        {
            foreach(Entity entity in entities)
            {
                SetEntityGoal(entity, goal);
            }
        }

        private float nextTickTime = 0;
        public void Update()
        {
            if(Time.time > nextTickTime)
            {
                nextTickTime = Time.time + (1f / (float)tickPerSecond);
                OnMovementTick();
            }
        }

        public virtual void OnMovementTick()
        {
            
        }
    }
}