using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{

    public class MainGrid : GridBase<MainGridChunk>
    {
        public MainGrid(int chunkDataSizeX, int chunkDataSizeY, Vector2 terrainSize)
            : base(
                  chunkDataSizeX, 
                  chunkDataSizeY, 
                  terrainSize.x / chunkDataSizeX,
                  terrainSize.y / chunkDataSizeY) { }
    }
}