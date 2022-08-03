using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BiomeWeightManager
    {
        public List<byte[,]> biomeWeightMap;
        public readonly byte biomeCount;
        public readonly Vector2Int size;
        private BiomesManager biomesManager;

        

        public BiomeWeightManager(BiomesManager biomesManager, Vector2Int size)
        {
            this.biomesManager = biomesManager;
            biomeCount = biomesManager.biomeCount;
            this.size = size;
            biomeWeightMap = new List<byte[,]>();
            for (int i = 0; i < biomeCount; i++)
                biomeWeightMap.Add(new byte[size.x, size.y]);

            
        }

        public BiomeWeightManager(BiomesManager biomesManager, int xsize, int ysize) : this(biomesManager, new Vector2Int(xsize, ysize)) { }

        public void SetWeight(BiomesType biomesType, int x, int y, float weight = 1f)
        {
            if (weight > 1f) weight = 1f;
            else if (weight < 0f) weight = 0f;
            biomeWeightMap[(byte)biomesType][x, y] = (byte)(weight * 255);
        }

        public float GetWeight(BiomesType biomesType, int x, int y)
        {
            return (float)biomeWeightMap[(byte)biomesType][x, y] / 255;
        }

        public Dictionary<BiomesType, float> GetWeight(int x, int y)
        {
            Dictionary<BiomesType, float> weights = new Dictionary<BiomesType, float>();
            for(byte i = 0; i < biomesManager.biomeCount; i++)
            {
                weights.Add((BiomesType)i, (float)biomeWeightMap[i][x, y] / 255);
            }

            return weights;
        }

        public void SetBiomeWeightMap(List<byte[,]> newWeightMap)
        {
            biomeWeightMap = newWeightMap;
        }
    }
}