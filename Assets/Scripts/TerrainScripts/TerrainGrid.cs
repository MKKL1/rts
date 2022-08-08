using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class TerrainCell
    {
        public BiomeType biome;
    }
    public class TerrainGrid
    {
        public TerrainCell[,] grid;
        public Vector2Int gridSize;
        /// <summary>
        /// Left bottom corner of grid in scene
        /// </summary>
        public Vector3 gridPosition;
        /// <summary>
        /// Size of single cell in scene
        /// </summary>
        public Vector2 cellSize;
        private Terrain terrain;


        public TerrainGrid(int terrainSizeX, int terrainSizeY, Terrain terrain)
        {
            this.terrain = terrain;
            gridSize = new Vector2Int(terrainSizeX, terrainSizeY);
            grid = new TerrainCell[terrainSizeX, terrainSizeY];

            gridPosition = terrain.transform.position;
            cellSize = new Vector2(terrain.terrainData.size.x/terrainSizeX, terrain.terrainData.size.y / terrainSizeY);

            for (int i = 0; i < gridSize.x; i++)
                for (int j = 0; j < gridSize.y; j++)
                {
                    grid[i, j] = new TerrainCell();
                }
        }

        public TerrainCell GetCellAtWorldPos(float x, float y)
        {
            return grid[Mathf.CeilToInt(x / cellSize.x), Mathf.CeilToInt(y / cellSize.y)];
        }
    }
}