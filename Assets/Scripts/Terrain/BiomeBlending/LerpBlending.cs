using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Terrain.Biomes;

namespace Assets.Scripts.Terrain.BiomeBlending
{
    public struct BlendingValues
    {
        public BiomeType minBiome;
        public BiomeType maxBiome;
        public float a;
    }
    public class LerpBlending : BiomeBlendingAlgorithm
    {
        private Dictionary<RangeAttribute, BlendingValues> blendingRanges;
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
            for (int x = 0; x < biomeAltitudeMap.GetLength(0); x++)
                for (int y = 0; y < biomeAltitudeMap.GetLength(1); y++)
                {
                    float height = biomeAltitudeMap[x, y];

                    //Search altitude range that height is in
                    foreach(KeyValuePair<RangeAttribute, BlendingValues> pair in blendingRanges)
                    {
                        if(Utils.inRange(pair.Key, height))
                        {
                            float weightMax = Utils.smoothStep((pair.Key.max - height) * pair.Value.a);
                            biomeWeightManager.SetWeight(pair.Value.maxBiome, x, y, weightMax);
                            biomeWeightManager.SetWeight(pair.Value.minBiome, x, y, 1f - weightMax);
                            
                            break;
                        }
                    }
                    
                }
        }

        

        private void createBlendingRanges()
        {
            if (biomeManager.biomeCount <= 1)
            {
                throw new Exception("Can only blend 2 or more biomes");
            }

            blendingRanges = new Dictionary<RangeAttribute, BlendingValues>();

            //Copy and sort biome list
            Biome[] sortedBiomes = new Biome[biomeManager.biomeList.Length];
            biomeManager.biomeList.CopyTo(sortedBiomes, 0);
            Array.Sort(sortedBiomes, (x, y) => x.biomeData.biomeAltitideMin.CompareTo(y.biomeData.biomeAltitideMin));

            //Create range where biomes have to be blended
            float lastMax = 0f;
            BiomeType lastBiomeType = BiomeType.WATER;
            foreach (Biome biome in sortedBiomes)
            {
                if (lastMax != 0f)
                {
                    float newMax = biome.biomeData.biomeAltitideMin + biome.biomeData.blendingValueStart;
                    blendingRanges.Add(new RangeAttribute(lastMax, newMax), new BlendingValues
                    {
                        minBiome = biome.biomeData.type,
                        maxBiome = lastBiomeType,
                        a = 1 / (newMax - lastMax)
                    });
                }
                lastMax = biome.biomeData.biomeAltitideMax - biome.biomeData.blendingValueEnd;
                lastBiomeType = biome.biomeData.type;
            }
        }
    }
}