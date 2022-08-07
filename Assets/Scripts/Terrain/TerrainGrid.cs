using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class TerrainCell
    {
        public BiomeType biome;
    }
    public class TerrainGrid
    {
        public TerrainCell[,] grid;
        public Vector2Int gridSize;

        public TerrainGrid(int chunksX, int chunksY)
        {
            gridSize = new Vector2Int(chunksX, chunksY);
            grid = new TerrainCell[chunksX, chunksY];
            for (int i = 0; i < gridSize.x; i++)
                for (int j = 0; j < gridSize.y; j++)
                {
                    grid[i, j] = new TerrainCell();
                }
        }
    }
}