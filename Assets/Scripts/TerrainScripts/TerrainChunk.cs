using Assets.Scripts.Networking;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class TerrainChunk : Chunk
    {
        public float[,] heightMap;
        public BiomeType[,] biomeGrid;
        public TerrainChunk(ushort chunkSizeX, ushort chunkSizeY) 
            : base(chunkSizeX, chunkSizeY) 
        {
            heightMap = new float[chunkSizeX, chunkSizeY];
            biomeGrid = new BiomeType[chunkSizeX, chunkSizeY];
        }

        //Compressing height map data
        public byte[,] GetHeightMap()
        {
            byte[,] bytes = new byte[chunkSizeX, chunkSizeY];
            for(int i = 0; i < chunkSizeX; i++)
                for(int j = 0; j < chunkSizeY; j++)
                {
                    bytes[i, j] = (byte)Mathf.Clamp(heightMap[i, j] * 255f, 0f, 255f);
                }
            return bytes;
        }

        public void SetHeightMap(byte[,] bytes)
        {
            for (int i = 0; i < chunkSizeX; i++)
                for (int j = 0; j < chunkSizeY; j++)
                {
                    heightMap[i, j] = Mathf.Clamp01((float)bytes[i, j] / 255f);
                }
        }
    }

    public static class TerrChunkNetReadWrite
    {
        public static void WriteTerrainChunk(this NetworkWriter networkWriter, TerrainChunk value)
        {
            networkWriter.WriteUShort(value.chunkSizeX);
            networkWriter.WriteUShort(value.chunkSizeY);
            networkWriter.WriteArray<byte>(value.GetHeightMap());
            networkWriter.WriteArray(value.biomeGrid);
        }

        public static TerrainChunk ReadTerrainChunk(this NetworkReader networkReader)
        {
            TerrainChunk terrainChunk = new TerrainChunk(networkReader.ReadUShort(), networkReader.ReadUShort());
            terrainChunk.SetHeightMap(networkReader.ReadArray<byte>(terrainChunk.chunkSizeX, terrainChunk.chunkSizeY));
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