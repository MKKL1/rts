using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class Biome
    {
        
        public string biomeName;
        public bool buildable = true;
        public float biomeStartPercentage = 0.9f;
        public virtual float GetHeight(float x, float y)
        {
            return 0f;
        }
    }
}