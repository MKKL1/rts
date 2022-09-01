using Assets.Scripts.TerrainScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    //Implementing easiest A* pathfinding
    public class PathFinding
    {
        private MainGrid mainGrid;
        private AStarAlgorithm aStarAlgorithm;
        public PathFinding(MainGrid mainGrid)
        {
            this.mainGrid = mainGrid;
            aStarAlgorithm = new AStarAlgorithm(mainGrid);
        }

        public MovementPath GetPath(Vector2 start, Vector2 goal)
        {
            Vector2Int startInt = new Vector2Int((int)(start.x / mainGrid.worldCellSize.x), (int)(start.y / mainGrid.worldCellSize.y));
            Vector2Int goalInt = new Vector2Int((int)(goal.x / mainGrid.worldCellSize.x), (int)(goal.y / mainGrid.worldCellSize.y));
            Stack<Vector2Int> pathInt = aStarAlgorithm.FindPath(startInt, goalInt);
            Queue<Vector2> points = new Queue<Vector2>(pathInt.Count);
            for(int i = 0;pathInt.Count > 0; i++)
            {
                points.Enqueue(pathInt.Pop() * mainGrid.worldCellSize);
            }
            return new MovementPath(points);
        }
    }
}