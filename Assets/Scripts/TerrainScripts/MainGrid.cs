using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class MainGrid
    {
        public TerrainResource[,] terrainResourceMap;
        public Vector2 cellSize;
        public Vector2Int size;

        private Vector3 terrainPos;
        public MainGrid(int sizeX, int sizeY)
        {
            size = new Vector2Int(sizeX, sizeY);
            terrainResourceMap = new TerrainResource[sizeX, sizeY];
        }

        public void CalculateCellSize(Terrain terrain)
        {
            cellSize = new Vector2(terrain.terrainData.size.x/size.x, terrain.terrainData.size.z / size.y);
            terrainPos = terrain.transform.position;
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(terrainPos.x + (x * cellSize.x), terrainPos.y + (y * cellSize.y));
        }
        

    }
}