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
        public float biomeStartPercentage = 0.09f;
        public BiomesType type;
        public RangeAttribute biomeAltitide;
        public float biomeBlendingValue;
        public virtual float GetHeight(float x, float y)
        {
            return 0f;
        }
    }
}