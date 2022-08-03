using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BiomeWeightManager
    {
        public List<float[,]> biomeWeightMap;
        public readonly byte biomeCount;
        public readonly Vector2Int size;
        private BiomesManager biomesManager;

        

        public BiomeWeightManager(BiomesManager biomesManager, Vector2Int size)
        {
            this.biomesManager = biomesManager;
            biomeCount = biomesManager.biomeCount;
            this.size = size;
            biomeWeightMap = new List<float[,]>();
            for (int i = 0; i < biomeCount; i++)
                biomeWeightMap.Add(new float[size.x, size.y]);

            
        }

        public BiomeWeightManager(BiomesManager biomesManager, int xsize, int ysize) : this(biomesManager, new Vector2Int(xsize, ysize)) { }

        public void SetWeight(BiomesType biomesType, int x, int y, float weight = 1f)
        {
            if (weight > 1f) weight = 1f;
            else if (weight < 0f) weight = 0f;
            biomeWeightMap[(byte)biomesType][x, y] = weight;
        }

        public float GetWeight(BiomesType biomesType, int x, int y)
        {
            return biomeWeightMap[(byte)biomesType][x, y];
        }

        public Dictionary<BiomesType, float> GetWeight(int x, int y)
        {
            Dictionary<BiomesType, float> weights = new Dictionary<BiomesType, float>();
            for(byte i = 0; i < biomesManager.biomeCount; i++)
            {
                weights.Add((BiomesType)i, biomeWeightMap[i][x, y]);
            }

            return weights;
        }

        public void SetBiomeWeightMap(List<float[,]> newWeightMap)
        {
            biomeWeightMap = newWeightMap;
        }
    }
}