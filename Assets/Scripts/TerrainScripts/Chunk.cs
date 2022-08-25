using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class Chunk
    {
        public ushort chunkSizeX;
        public ushort chunkSizeY;
        public Chunk(ushort chunkSizeX, ushort chunkSizeY)
        {
            this.chunkSizeX = chunkSizeX;
            this.chunkSizeY = chunkSizeY;
        }
    }
}