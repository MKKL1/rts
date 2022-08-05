using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class Biome
    {
        public string biomeName;
        public Color biomeColor;
        public bool buildable = true;
        public bool walkable = true;
        public BiomeType type;
        public RangeAttribute biomeAltitide;
        public float blendingValueStart = 0.05f;
        public float blendingValueEnd = 0.05f;
        public virtual float GetHeight(float x, float y)
        {
            return 0f;
        }
    }
}