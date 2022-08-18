using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public struct TerrainGrid
    {
        public BiomeType[,] biomeGrid;
        public Vector2Int gridSize;
        /// <summary>
        /// Size of single cell in scene
        /// </summary>
        public Vector2 cellSize;


        public TerrainGrid(int gridSizeX, int gridSizeY, float cellSizeX, float cellSizeY)
        {
            gridSize = new Vector2Int(gridSizeX, gridSizeY);
            biomeGrid = new BiomeType[gridSizeX, gridSizeY];
            cellSize = new Vector2(cellSizeX, cellSizeY);

            //cellSize = new Vector2(terrain.terrainData.size.x/terrainSizeX, terrain.terrainData.size.z / terrainSizeY);
        }

        public Vector2 GetTerrainWorldSize()
        {
            return gridSize * cellSize;
        }

        public BiomeType GetCellAtWorldPos(float x, float y)
        {
            return biomeGrid[Mathf.CeilToInt(x / cellSize.x), Mathf.CeilToInt(y / cellSize.y)];
        }

        public BiomeType GetCellAtWorldPos(Vector2 vector)
        {
            return GetCellAtWorldPos(vector.x, vector.y);
        }
    }
}