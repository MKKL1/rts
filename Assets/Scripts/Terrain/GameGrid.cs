using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class GameGrid
    {
        public GridElement[,] grid;
        public Vector2Int gridSize;

        public GameGrid(int chunksX, int chunksY)
        {
            gridSize = new Vector2Int(chunksX, chunksY);
            grid = new GridElement[chunksX, chunksY];
            for (int i = 0; i < gridSize.x; i++)
                for (int j = 0; j < gridSize.y; j++)
                {
                    grid[i, j] = new GridElement();
                }
        }
    }
}