using Assets.Scripts.Terrain.Biomes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class BiomesManager
    {
        private Dictionary<BiomesType, Biome> biomeList = new Dictionary<BiomesType, Biome>();
        public BiomesManager(int seed)
        {
            biomeList.Add(BiomesType.PLAINS, new Plains(seed));
            biomeList.Add(BiomesType.WATER, new Water(seed));
        }

        public BiomesType GetBiomeTypeFromHeight(float height)
        {
            return biomeList.FirstOrDefault(x => inRange(x.Value.biomeAltitide, height)).Key;
        }

        public Biome GetBiomeFromBiomeType(BiomesType biomesType)
        {
            return biomeList.GetValueOrDefault(biomesType);
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