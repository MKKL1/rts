using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public struct ChunkIteratorStep<T> where T : Chunk
    {
        public T currentChunk;
        public int xInChunk;
        public int yInChunk;
        public int chunkX;
        public int chunkY;
    }
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
        public Vector2Int gridDataSize { get; internal set; }
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
            get { return gridDataSize * worldCellSize; }
        }
        /// <summary>
        /// Size of single cell of grid in world space
        /// </summary>
        public Vector2 worldCellSize { get; internal set; }
        public readonly int chunkSize = GlobalConfig.CHUNK_SIZE;

        public GridBase(Vector2Int chunkDataSize, Vector2 cellSize)
        {
            this.gridDataSize = chunkDataSize;
            this.worldCellSize = cellSize;
            //TODO WTF
            this.chunkArrayLength = new Vector2Int(
                Mathf.CeilToInt((float)chunkDataSize.x / chunkSize),
                Mathf.CeilToInt((float)chunkDataSize.y / chunkSize));
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
            return chunks[x / chunkSize, y / chunkSize];
        }

        protected Vector2Int GetInChunkOffset(int x, int y)
        {
            return new Vector2Int(x % chunkSize, y % chunkSize);
        }
        /// <param name="x">x in world space</param>
        /// <param name="y">z in world space</param>
        /// <returns>Returns chunk given by world space position</returns>
        public T GetChunkAtWorldPos(float x, float y)
        {
            return chunks[
                Mathf.FloorToInt(x / (chunkSize * worldCellSize.x)),
                Mathf.FloorToInt(y / (chunkSize * worldCellSize.y))];
        }

        protected Vector2Int GetWorldPosInChunkOffset(float x, float y)
        {
            return new Vector2Int(
                 Mathf.FloorToInt(x % (chunkSize * worldCellSize.x)),
                 Mathf.FloorToInt(y % (chunkSize * worldCellSize.y)));
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

        public Vector2Int GetChunkSizeAt(int x, int y)
        {
            int sizeX = chunkSize;
            if (x == chunkArrayLength.x - 1)
            {
                sizeX = gridDataSize.x % chunkSize;
                if (sizeX == 0) sizeX = chunkSize;
                sizeX--;
            }

            int sizeY = chunkSize;
            if (y == chunkArrayLength.y - 1)
            {
                sizeY = gridDataSize.y % chunkSize;
                if (sizeY == 0) sizeY = chunkSize;
                sizeY--;
            }
            return new Vector2Int(sizeX, sizeY);
        }
        ///// <summary>
        ///// Iterates grid faster by accessing chunk then iterating inside it
        ///// </summary>
        //public void IterateGrid(Action<ChunkIteratorStep<T>> action, int xSize = -1, int ySize = -1)
        //{
        //    if (xSize == -1) xSize = chunkDataSize.x;
        //    if (ySize == -1) ySize = chunkDataSize.y;
        //    int chunkXSize = Mathf.CeilToInt(xSize / chunkArrayLength.x);
        //    int chunkYSize = Mathf.CeilToInt(ySize / chunkArrayLength.y);

        //    int lastX = xSize % GlobalConfig.CHUNK_SIZE;
        //    if (lastX == 0) lastX = GlobalConfig.CHUNK_SIZE;

        //    int lastY = ySize % GlobalConfig.CHUNK_SIZE;
        //    if (lastY == 0) lastY = GlobalConfig.CHUNK_SIZE;

        //    for (int chunkX = 0; chunkX < chunkXSize; chunkX++)
        //        for (int chunkY = 0; chunkY < chunkYSize; chunkY++)
        //        {
        //            int currentXSize = chunkX == chunkXSize - 1 ? lastX : GlobalConfig.CHUNK_SIZE;
        //            int currentYSize = chunkY == chunkYSize - 1 ? lastY : GlobalConfig.CHUNK_SIZE;

        //            T currentChunk = chunks[chunkX, chunkY];
        //            for (int inChunkX = 0; inChunkX < currentXSize; inChunkX++)
        //                for (int inChunkY = 0; inChunkY < currentYSize; inChunkY++)
        //                {
        //                    action.Invoke(new ChunkIteratorStep<T>()
        //                    {
        //                        currentChunk = currentChunk,
        //                        xInChunk = inChunkX,
        //                        yInChunk = inChunkY,
        //                        chunkX = chunkX,
        //                        chunkY = chunkY
        //                    });
        //                }
        //        }
        //}

        public void IterateChunks(Action<int, int> action)
        {
            for(int x = 0; x < chunkArrayLength.x; x++)
                for(int y = 0; y < chunkArrayLength.y; y++)
                {
                    action.Invoke(x, y);
                }
        }

        public void IterateChunks(Action<T> action)
        {
            IterateChunks(new Action<int, int>((x,y) => action.Invoke(chunks[x, y])));
        }

        public void IterateInChunk(T chunk, Action<int,int> action)
        {
            for(int x = 0; x < chunk.chunkSizeX; x++)
                for(int y = 0; y < chunk.chunkSizeY; y++)
                {
                    action.Invoke(x, y);
                }
        }

        public void IterateInChunk(int chunkX, int chunkY, Action<int, int> action)
        {
            T chunk = chunks[chunkX, chunkY];
            IterateInChunk(chunk, action);
        }

        public Task[] IterateChunksAsync(Action<int, int> action)
        {
            List<Task> taskList = new List<Task>();
            Action<int, int> newThreadAction = (x,y) =>
            {
                taskList.Add(Task.Factory.StartNew(() => action.Invoke(x,y)));
            };
            return taskList.ToArray();
        }

        public Task[] IterateChunksAsync(Action<T> action)
        {
            return IterateChunksAsync(new Action<int, int>((x, y) => action.Invoke(chunks[x, y])));
        }

        //TODO
        //public void IterateGrid(Action<ChunkIteratorStep<T>> action) { }
    }
}