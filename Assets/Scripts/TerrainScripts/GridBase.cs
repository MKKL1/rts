using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class GridBase<T> where T : Chunk
    {
        public T[,] chunks;
        /// <summary>
        /// Size of chunk array (new chunks[chunkArrayLength.x, chunkArrayLength.y])
        /// </summary>
        public Vector2Int chunkArrayLength { get; internal set; }
        /// <summary>
        /// Size of data that was split into chunks.
        /// Some chunks can be not fully filled with data
        /// </summary>
        public Vector2Int chunkDataSize { get; internal set; }
        /// <summary>
        /// Size of single chunk in world space
        /// </summary>
        public Vector2 worldChunkSize
        {
            get { return worldCellSize * GlobalConfig.CHUNK_SIZE; }
        }
        /// <summary>
        /// Size of entire grid in world space
        /// </summary>
        public Vector2 worldGridSize
        {
            get { return chunkArrayLength * worldChunkSize; }
        }
        /// <summary>
        /// Size of single cell of grid in world space
        /// </summary>
        public Vector2 worldCellSize { get; internal set; }

        public GridBase(Vector2Int chunkDataSize, Vector2 cellSize)
        {
            this.chunkDataSize = chunkDataSize;
            this.worldCellSize = cellSize;
            this.chunkArrayLength = chunkDataSize / GlobalConfig.CHUNK_SIZE;
            chunks = new T[chunkArrayLength.x, chunkArrayLength.y];
        }

        public GridBase(int chunkDataSizeX, int chunkDataSizeY, float cellSizeX, float cellSizeY)
            : this(new Vector2Int(chunkDataSizeX, chunkDataSizeY), new Vector2(cellSizeX, cellSizeY)) { }
        /// <summary>
        /// Returns chunk relative to grid coordinates
        /// E.g. if chunk array size is 4x4 and chunk size is 100 then GetChunkAt(150, 50) will return chunkArray[1,0]
        /// </summary>
        /// <param name="x">x on grid</param>
        /// <param name="y">y on grid</param>
        /// <returns>Chunk at x,y</returns>
        public T GetChunkAt(int x, int y)
        {
            return chunks[x / GlobalConfig.CHUNK_SIZE, y / GlobalConfig.CHUNK_SIZE];
        }

        protected Vector2Int GetInChunkOffset(int x, int y)
        {
            return new Vector2Int(x % GlobalConfig.CHUNK_SIZE, y % GlobalConfig.CHUNK_SIZE);
        }
        /// <param name="x">x in world space</param>
        /// <param name="y">z in world space</param>
        /// <returns>Returns chunk given by world space position</returns>
        public T GetChunkAtWorldPos(float x, float y)
        {
            return chunks[
                Mathf.FloorToInt(x / (GlobalConfig.CHUNK_SIZE * worldCellSize.x)),
                Mathf.FloorToInt(y / (GlobalConfig.CHUNK_SIZE * worldCellSize.y))];
        }

        protected Vector2Int GetWorldPosInChunkOffset(float x, float y)
        {
            return new Vector2Int(
                 Mathf.FloorToInt(x % (GlobalConfig.CHUNK_SIZE * worldCellSize.x)),
                 Mathf.FloorToInt(y % (GlobalConfig.CHUNK_SIZE * worldCellSize.y)));
        }

        /// <summary>
        /// World space position of grid node given by x and y
        /// </summary>
        /// <param name="x">x of grid node</param>
        /// <param name="y">y of grid node</param>
        /// <returns>World position of grid node</returns>
        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(x * worldCellSize.x, y * worldCellSize.y);
        }

        /// <summary>
        /// Get position of grid node given by world space position
        /// </summary>
        /// <param name="x">x in world space position</param>
        /// <param name="y">z in world space position</param>
        /// <returns>Position of main grid node</returns>
        public Vector2Int GetGridPostion(float x, float y)
        {
            return new Vector2Int(Mathf.FloorToInt(x / worldCellSize.x), Mathf.FloorToInt(y / worldCellSize.y));
        }
        /// <summary>
        /// Get position of grid node given by world space position
        /// </summary>
        /// <param name="worldPosition">world space position (y is ignored)</param>
        /// <returns>Position of main grid node</returns>
        public Vector2Int GetGridPostion(Vector3 worldPosition)
        {
            return GetGridPostion(worldPosition.x, worldPosition.z);
        }
    }
}