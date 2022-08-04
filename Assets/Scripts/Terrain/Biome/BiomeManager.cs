using Assets.Scripts.Terrain.Biomes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BiomesManager
    {
        public readonly byte biomeCount = 3;
        public readonly Biome[] biomeList;
        public BiomesManager(int seed)
        {
            biomeList = new Biome[biomeCount];
            biomeList[(byte)BiomeType.PLAINS] = new Plains(seed);
            biomeList[(byte)BiomeType.WATER] = new Water(seed);
            biomeList[(byte)BiomeType.BEACH] = new Beach(seed);
        }

        public BiomeType GetBiomeType(float height)
        {
            for(byte i = 0; i < biomeCount; i++)
            {
                if (biomeList[i] != null && inRange(biomeList[i].biomeAltitide, height)) return (BiomeType)i;
            }
            return BiomeType.WATER;
        }

        public Biome GetBiome(BiomeType biomeType)
        {
            return biomeList[(byte)biomeType];
        }

        public Color GetBiomeColor(BiomeType biomeType)
        {
            return GetBiome(biomeType).biomeColor;
        }

        private bool inRange(RangeAttribute range, float value)
        {
            return value >= range.min && value <= range.max;
        }
    }

    public enum BiomeType : byte
    {
        PLAINS,
        WATER,
        BEACH
    }
}