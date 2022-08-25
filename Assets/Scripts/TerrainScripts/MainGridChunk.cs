using Assets.Scripts.TerrainScripts.Details;
using Mirror;
using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class MainGridChunk : Chunk
    {
        public bool[,] walkableMap;
        public TerrainResourceNode[,] resourceMap;
        public MainGridChunk(ushort chunkSizeX, ushort chunkSizeY)
            : base(chunkSizeX, chunkSizeY)
        {
            walkableMap = new bool[chunkSizeX, chunkSizeY];
            
            resourceMap = new TerrainResourceNode[chunkSizeX, chunkSizeY];
        }
    }

    public static class MainGChunkNetReadWrite
    {
        public static void WriteMainGridChunk(this NetworkWriter networkWriter, MainGridChunk value)
        {
            networkWriter.WriteUShort(value.chunkSizeX);
            networkWriter.WriteUShort(value.chunkSizeY);
            networkWriter.WriteArray(value.walkableMap);
            networkWriter.WriteArray(value.resourceMap);
        }

        public static MainGridChunk ReadMainGridChunk(this NetworkReader networkReader)
        {
            MainGridChunk chunk = new MainGridChunk(networkReader.ReadUShort(), networkReader.ReadUShort());
            chunk.walkableMap = networkReader.ReadArray<bool>(chunk.chunkSizeX, chunk.chunkSizeY);
            chunk.resourceMap = networkReader.ReadArray<TerrainResourceNode>(chunk.chunkSizeX, chunk.chunkSizeY);
            return chunk;
        }
    }
}