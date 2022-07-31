using UnityEngine;

namespace Assets
{
    public class GameGrid
    {
        public GridElement[,] grid;
        public Vector2Int gridSize;
        public Vector2Int chunkGridSize;

        public GameGrid(int chunksX, int chunksY, int chunkSizeX = 10, int chunkSizeY = 10)
        {
            gridSize = new Vector2Int(chunksX, chunksY);
            chunkGridSize = new Vector2Int(chunkSizeX, chunkSizeY);
        }
    }
}