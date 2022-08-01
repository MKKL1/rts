using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Biome
    {
        
        public string biomeName;
        public bool buildable = true;
        public bool walkable = true;
        public float biomeStartPercentage = 0.09f;
        public virtual float GetHeight(float x, float y)
        {
            return 0f;
        }
    }
}