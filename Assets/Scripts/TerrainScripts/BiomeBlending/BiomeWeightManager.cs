using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class BiomeWeightManager
    {
        public float[,,] biomeWeightMap;
        public readonly byte biomeCount;
        public readonly Vector2Int size;
        private BiomesManager biomesManager;



        public BiomeWeightManager(BiomesManager biomesManager, Vector2Int size)
        {
            this.biomesManager = biomesManager;
            biomeCount = biomesManager.biomeCount;
            this.size = size;
            biomeWeightMap = new float[biomeCount,size.x, size.y];


        }

        public BiomeWeightManager(BiomesManager biomesManager, int xsize, int ysize) : this(biomesManager, new Vector2Int(xsize, ysize)) { }

        public void SetWeight(BiomeType biomeType, int x, int y, float weight = 1f)
        {
            if (weight > 1f) weight = 1f;
            else if (weight < 0f) weight = 0f;
            biomeWeightMap[(byte)biomeType,x, y] = weight;
        }

        public float GetWeight(BiomeType biomeType, int x, int y)
        {
            return biomeWeightMap[(byte)biomeType,x, y];
        }

        public Dictionary<BiomeType, float> GetWeight(int x, int y)
        {
            Dictionary<BiomeType, float> weights = new Dictionary<BiomeType, float>();
            for (byte i = 0; i < biomesManager.biomeCount; i++)
            {
                weights.Add((BiomeType)i, biomeWeightMap[i,x, y]);
            }

            return weights;
        }

        public void SetBiomeWeightMap(float[,,] newWeightMap)
        {
            biomeWeightMap = newWeightMap;
        }
    }
}