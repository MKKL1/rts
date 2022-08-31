using Assets.Scripts.TerrainScripts;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    //Implementing easiest A* pathfinding
    public class PathFinding
    {
        private MainGrid mainGrid;
        public PathFinding(MainGrid mainGrid)
        {
            this.mainGrid = mainGrid;
        }

        public MovementPath GetPath(Vector2 start, Vector2 goal)
        {

            return default;
        }
    }
}