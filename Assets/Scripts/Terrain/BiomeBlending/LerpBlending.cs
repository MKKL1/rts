using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain.BiomeBlending
{
    public class LerpBlending : BiomeBlendingAlgorithm
    {
        private Dictionary<RangeAttribute, BiomesType[]> blendingRanges;
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

        }

        private void createBlendingRanges()
        {
            if (biomeManager.biomeCount <= 1)
            {
                throw new System.Exception("Can only blend 2 or more biomes");
            }

            blendingRanges = new Dictionary<RangeAttribute, BiomesType[]>();
            Biome[] sortedBiomes = new Biome[biomeManager.biomeList.Length];
            biomeManager.biomeList.CopyTo(sortedBiomes, 0);
            Array.Sort(sortedBiomes, (x, y) => x.biomeAltitide.min.CompareTo(y.biomeAltitide.min));

            float lastMax = 0f;
            BiomesType lastBiomeType = BiomesType.WATER;
            foreach (Biome biome in sortedBiomes)
            {
                if (lastMax != 0f)
                {
                    blendingRanges.Add(new RangeAttribute(lastMax, biome.biomeAltitide.min + biome.biomeBlendingValue), new BiomesType[] { biome.type, lastBiomeType });
                }
                lastMax = biome.biomeAltitide.max - biome.biomeBlendingValue;
                lastBiomeType = biome.type;
            }
        }
    }
}