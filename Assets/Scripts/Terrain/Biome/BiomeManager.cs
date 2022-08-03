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
            biomeList[(byte)BiomesType.PLAINS] = new Plains(seed);
            biomeList[(byte)BiomesType.WATER] = new Water(seed);
            biomeList[(byte)BiomesType.BEACH] = new Beach(seed);
        }

        public BiomesType GetBiomeType(float height)
        {
            for(byte i = 0; i < biomeCount; i++)
            {
                if (biomeList[i] != null && inRange(biomeList[i].biomeAltitide, height)) return (BiomesType)i;
            }
            return BiomesType.WATER;
        }

        public Biome GetBiome(BiomesType biomesType)
        {
            return biomeList[(byte)biomesType];
        }

        public Color GetBiomeColor(BiomesType biomesType)
        {
            return GetBiome(biomesType).biomeColor;
        }

        private bool inRange(RangeAttribute range, float value)
        {
            return value >= range.min && value <= range.max;
        }
    }

    public enum BiomesType : byte
    {
        PLAINS,
        WATER,
        BEACH
    }
}