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
        }
    }
}