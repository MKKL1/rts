using Assets.Scripts.Networking;
using Assets.Scripts.TerrainScripts.Details;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation
{
    public struct TerrainGeneratorResult : NetworkMessage
    {
        public Vector2Int mainGridSize;
        public bool[,] walkableMap;
        public Vector2 terrainSize;
        public Vector2Int terrainGridSize;
        public BiomeType[,] biomeGrid;
        public byte[,] heightMap;
        public TerrainResourceNode[,] resourceMap;
    }

    public static class GenResultWriteRead 
    {
        public static void WriteTerrainGenResult(this NetworkWriter networkWriter, TerrainGeneratorResult value)
        {
            networkWriter.WriteVector2Int(value.mainGridSize);
            networkWriter.WriteArray(value.walkableMap);
            networkWriter.WriteVector2(value.terrainSize);
            networkWriter.WriteVector2Int(value.terrainGridSize);
            networkWriter.WriteArray(value.biomeGrid);
            networkWriter.WriteArray(value.heightMap);
            networkWriter.WriteArray(value.resourceMap);
        }

        public static TerrainGeneratorResult ReadTerrainGenResult(this NetworkReader networkReader)
        {
            TerrainGeneratorResult value = new TerrainGeneratorResult();
            value.mainGridSize = networkReader.ReadVector2Int();
            value.walkableMap = networkReader.ReadArray<bool>((uint)value.mainGridSize.x, (uint)value.mainGridSize.y);
            value.terrainSize = networkReader.ReadVector2();
            value.terrainGridSize = networkReader.ReadVector2Int();
            value.biomeGrid = networkReader.ReadArray<BiomeType>((uint)value.terrainGridSize.x, (uint)value.terrainGridSize.y);
            value.heightMap = networkReader.ReadArray<byte>((uint)value.terrainGridSize.x, (uint)value.terrainGridSize.y);
            value.resourceMap = networkReader.ReadArray<TerrainResourceNode>((uint)value.terrainGridSize.x, (uint)value.terrainGridSize.y);
            return value;
        }
    }
}