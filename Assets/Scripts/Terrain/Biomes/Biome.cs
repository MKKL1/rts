using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Biome
    {
        public BiomeData biomeData;
        public Biome(string biomeFileName)
        {
            biomeData = (BiomeData)AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/BiomeDefinition/{biomeFileName}.asset", typeof(BiomeData));
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