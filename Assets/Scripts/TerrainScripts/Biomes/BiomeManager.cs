using Assets.Scripts.TerrainScripts.Biomes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class BiomeManager
    {
        public byte biomeCount { get; internal set; } = 0;
        public Biome[] biomeList { get; internal set; }

        private List<Biome> initialBiomeList = new List<Biome>();

        public void addBiome(Biome biome)
        {
            biomeCount++;
            initialBiomeList.Add(biome);
        }

        public void initBiomes()
        {
            biomeList = new Biome[biomeCount];
            for(int i = 0; i < initialBiomeList.Count; i++)
            {
                biomeList[(byte)initialBiomeList[i].biomeData.type] = initialBiomeList[i];
            }
        }

        public BiomeType GetBiomeType(float height)
        {
            for(byte i = 0; i < biomeCount; i++)
            {
                if (biomeList[i] != null && Utils.inRange(biomeList[i].biomeData.biomeAltitideMin, biomeList[i].biomeData.biomeAltitideMax, height)) return (BiomeType)i;
            }
            return BiomeType.WATER;
        }

        public Biome GetBiome(BiomeType biomeType)
        {
            return biomeList[(byte)biomeType];
        }

        public Color GetBiomeColor(BiomeType biomeType)
        {
            return GetBiome(biomeType).biomeData.biomeColor;
        }


    }

    public enum BiomeType : byte
    {
        PLAINS,
        WATER,
        BEACH,
        MOUNTAINS
    }
}