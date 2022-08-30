using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Biomes
{
    public class Biome
    {
        public BiomeData biomeData;
        public Biome(BiomeData biomeData)
        {
            this.biomeData = biomeData;
        }
        public virtual float GetHeight(float x, float y)
        {
            return 0f;
        }

        public virtual GameObject GetFeature(float x, float y)
        {
            return null;
        }
    }
}