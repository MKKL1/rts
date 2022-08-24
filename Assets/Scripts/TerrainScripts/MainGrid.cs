using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{

    public struct MainGrid
    {
        public TerrainResource[,] terrainResourceMap;
        public bool[,] walkable;
        public Vector2 cellSize;
        public Vector2Int size;

        private Vector3 terrainPos;
        public MainGrid(Vector2Int size, Vector2 terrainSize)
        {
            this.size = size;
            terrainResourceMap = new TerrainResource[size.x, size.y];
            walkable = new bool[size.x, size.y];
            Utils.SetArrayTo(ref walkable, true);
            cellSize = terrainSize / size;
            terrainPos = new Vector3(0, 0, 0);
        }

        public MainGrid(int sizeX, int sizeY, Vector2 terrainSize) : this(new Vector2Int(sizeX, sizeY), terrainSize) { }

        public MainGrid(int sizeX, int sizeY, float terrainSizeX, float terrainSizeY) : this(sizeX, sizeY, new Vector2(terrainSizeX, terrainSizeY)) {}

        /// <summary>
        /// World position of grid node given by x and y
        /// </summary>
        /// <param name="x">x of grid node</param>
        /// <param name="y">y of grid node</param>
        /// <returns>World position of grid node</returns>
        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(terrainPos.x + (x * cellSize.x), terrainPos.y + (y * cellSize.y));
        }

        /// <summary>
        /// Get position of grid node given by world position
        /// </summary>
        /// <param name="x">x in world position</param>
        /// <param name="y">z in world position</param>
        /// <returns>Position of main grid node</returns>
        public Vector2Int GetGridPostion(float x, float y)
        {
            return new Vector2Int(Mathf.FloorToInt(x / cellSize.x), Mathf.FloorToInt(y / cellSize.y));
        }

        public Vector2Int GetGridPostion(Vector3 worldPosition)
        {
            return GetGridPostion(worldPosition.x, worldPosition.z);
        }

        public Texture2D GetWalkable()
        {
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
            for(int i = 0; i < size.x; i++)
                for(int j = 0; j < size.y; j++)
                {
                    if (walkable[i, j])
                        texture.SetPixel(i, j, Color.white);
                    else texture.SetPixel(i, j, Color.black);
                }
            texture.Apply();
            return texture;
        }
        

    }
}