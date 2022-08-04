using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain.BiomeBlending
{
    public class LerpBlending : BiomeBlendingAlgorithm
    {
        private Dictionary<RangeAttribute, BiomeType[]> blendingRanges;
        private BiomesManager biomeManager;
        private float[,] biomeAltitudeMap;
        public LerpBlending(BiomesManager biomeManager, float[,] biomeAltitudeMap)
        {
            this.biomeManager = biomeManager;
            this.biomeAltitudeMap = biomeAltitudeMap;
        }

        public void blendBiomes(ref BiomeWeightManager biomeWeightManager)
        {
            createBlendingRanges();
            //TODO Can be async
            //for (int x = 0; x < biomeAltitudeMap.GetLength(0); x++)
            //    for (int y = 0; y < biomeAltitudeMap.GetLength(1); y++)
            //    {
            //        blendingRanges
            //    }
        }

        private void createBlendingRanges()
        {
            if (biomeManager.biomeCount <= 1)
            {
                throw new System.Exception("Can only blend 2 or more biomes");
            }

            blendingRanges = new Dictionary<RangeAttribute, BiomeType[]>();
            Biome[] sortedBiomes = new Biome[biomeManager.biomeList.Length];
            biomeManager.biomeList.CopyTo(sortedBiomes, 0);
            Array.Sort(sortedBiomes, (x, y) => x.biomeAltitide.min.CompareTo(y.biomeAltitide.min));

            float lastMax = 0f;
            BiomeType lastBiomeType = BiomeType.WATER;
            foreach (Biome biome in sortedBiomes)
            {
                if (lastMax != 0f)
                {
                    blendingRanges.Add(new RangeAttribute(lastMax, biome.biomeAltitide.min + biome.biomeBlendingValue), new BiomeType[] { biome.type, lastBiomeType });
                }
                lastMax = biome.biomeAltitide.max - biome.biomeBlendingValue;
                lastBiomeType = biome.type;
            }
        }
    }
}