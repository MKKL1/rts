using Assets.Scripts.Networking;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class TerrainChunk : Chunk
    {
        public byte[,] heightMap;
        public TerrainChunk(ushort chunkSizeX, ushort chunkSizeY) 
            : base(chunkSizeX, chunkSizeY) 
        {
            heightMap = new byte[chunkSizeX, chunkSizeY];
        }
    }

    public static class TerrChunkNetReadWrite
    {
        public static void WriteTerrainChunk(this NetworkWriter networkWriter, TerrainChunk value)
        {
            networkWriter.WriteUShort(value.chunkSizeX);
            networkWriter.WriteUShort(value.chunkSizeY);
            networkWriter.WriteArray(value.heightMap);
        }

        public static TerrainChunk ReadTerrainChunk(this NetworkReader networkReader)
        {
            TerrainChunk terrainChunk = new TerrainChunk(networkReader.ReadUShort(), networkReader.ReadUShort());
            terrainChunk.heightMap = networkReader.ReadArray<byte>(terrainChunk.chunkSizeX, terrainChunk.chunkSizeY);
            return terrainChunk;
        }
    }
}