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
        public Vector2 terrainSize;
        public Vector2Int terrainGridSize;
        public BiomeType[,] biomeGrid;
        public byte[,] heightMap;
        public TerrainResourceNode[,] resourceMap;
    }
}