using Assets.Scripts.Terrain.Biomes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BiomesManager
    {
        byte biomeCount = 4;
        private Biome[] biomeList;
        public BiomesManager(int seed)
        {
            biomeList = new Biome[biomeCount];
            biomeList[(byte)BiomesType.PLAINS] = new Plains(seed);
            biomeList[(byte)BiomesType.WATER] = new Water(seed);
        }

        public BiomesType GetBiomeTypeFromHeight(float height)
        {
            for(byte i = 0; i < biomeCount; i++)
            {
                if (inRange(biomeList[i].biomeAltitide, height)) return (BiomesType)i;
            }
            return BiomesType.WATER;
        }

        public Biome GetBiomeFromBiomeType(BiomesType biomesType)
        {
            return biomeList[(byte)biomesType];
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
        MOUNTAINS,
        BEACH
    }
}