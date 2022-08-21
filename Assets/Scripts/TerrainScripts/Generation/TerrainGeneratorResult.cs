using Assets.Scripts.Networking;
using Assets.Scripts.TerrainScripts.Details;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation
{
    public class TerrainGeneratorResult : NetworkMessage
    {
        //TODO temporary sending entire MainGrid
        public MainGrid mainGrid;
        public BiomeType[,] biomeGrid;
        public byte[,] heightMap;
        public TerrainResourceNode[,] resourceMap;

        public void Write(NetworkWriter networkWriter)
        {
            networkWriter.WriteVector2Int(mainGrid.size);
            networkWriter.WriteArray(biomeGrid);
        }
    }
}