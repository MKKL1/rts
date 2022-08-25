using Assets.Scripts.Networking;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class TerrainChunk : Chunk
    {
        public byte[,] heightMap;
        public BiomeType[,] biomeGrid;
        public TerrainChunk(ushort chunkSizeX, ushort chunkSizeY) 
            : base(chunkSizeX, chunkSizeY) 
        {
            heightMap = new byte[chunkSizeX, chunkSizeY];
            biomeGrid = new BiomeType[chunkSizeX, chunkSizeY];
        }
    }

    public static class TerrChunkNetReadWrite
    {
        public static void WriteTerrainChunk(this NetworkWriter networkWriter, TerrainChunk value)
        {
            networkWriter.WriteUShort(value.chunkSizeX);
            networkWriter.WriteUShort(value.chunkSizeY);
            networkWriter.WriteArray(value.heightMap);
            networkWriter.WriteArray(value.biomeGrid);
        }

        public static TerrainChunk ReadTerrainChunk(this NetworkReader networkReader)
        {
            TerrainChunk terrainChunk = new TerrainChunk(networkReader.ReadUShort(), networkReader.ReadUShort());
            terrainChunk.heightMap = networkReader.ReadArray<byte>(terrainChunk.chunkSizeX, terrainChunk.chunkSizeY);
            terrainChunk.biomeGrid = networkReader.ReadArray<BiomeType>(terrainChunk.chunkSizeX, terrainChunk.chunkSizeY);
            return terrainChunk;
        }


        //TODO for some reason enum has to be treated like custom type
        public static void WriteBiomeType(this NetworkWriter networkWriter, BiomeType value)
        {
            networkWriter.WriteByte((byte)value);
        }

        public static BiomeType ReadBiomeType(this NetworkReader networkReader)
        {
            return (BiomeType)networkReader.ReadByte();
        }
    }
}