using Assets.Scripts.DebugTools;
using Assets.Scripts.TerrainScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    //Implementing easiest A* pathfinding
    public class PathFinding
    {
        private MainGrid mainGrid;
        private AStarAlgorithm aStarAlgorithm;
        private NativeArray<bool> walkableMap;
        public PathFinding(MainGrid mainGrid)
        {
            this.mainGrid = mainGrid;
            walkableMap = new NativeArray<bool>(mainGrid.gridDataSize.x * mainGrid.gridDataSize.y, Allocator.Persistent);
            mainGrid.IterateChunks(new System.Action<int, int>((xChunk, yChunk) =>
            {
                MainGridChunk chunk = mainGrid.chunks[xChunk, yChunk];
                mainGrid.IterateInChunk(chunk, new System.Action<int, int>((xInChunk, yInChunk) =>
                {
                    int xInGrid = (xChunk * mainGrid.chunkSize) + xInChunk;
                    int yInGrid = (yChunk * mainGrid.chunkSize) + yInChunk;
                    walkableMap[xInGrid + (yInGrid * mainGrid.gridDataSize.y)] = chunk.walkableMap[xInChunk, yInChunk];
                }));
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns>null if path was not found</returns>
        public MovementPath GetPath(Vector2 start, Vector2 goal)
        {
            Vector2Int startInt = new Vector2Int((int)(start.x / mainGrid.worldCellSize.x), (int)(start.y / mainGrid.worldCellSize.y));
            Vector2Int goalInt = new Vector2Int((int)(goal.x / mainGrid.worldCellSize.x), (int)(goal.y / mainGrid.worldCellSize.y));
            aStarAlgorithm = new AStarAlgorithm(walkableMap, new Unity.Mathematics.int2(mainGrid.gridDataSize.x, mainGrid.gridDataSize.y));
            Stack<Vector2Int> pathInt = aStarAlgorithm.FindPath(startInt, goalInt);
            if (pathInt == null) return null;
            Debug.Log($"Path generated with length of {pathInt.Count}");
            Queue<Vector2> points = new Queue<Vector2>(pathInt.Count);
            for(int i = 0;pathInt.Count > 0; i++)
            {
                points.Enqueue((pathInt.Pop() + new Vector2(0.5f, 0.5f)) * mainGrid.worldCellSize);
            }
            return new MovementPath(points);
        }
    }
}