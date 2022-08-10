using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Biomes
{
    [CreateAssetMenu]
    public class BiomeData : ScriptableObject
    {
        public string biomeName;
        public Color biomeColor;
        public bool buildable = true;
        public bool walkable = true;
        public bool resources = true;
        public BiomeType type;
        public float biomeAltitideMin;
        public float biomeAltitideMax;
        public float blendingValueStart = 0.05f;
        public float blendingValueEnd = 0.05f;
        public float heightMultiplier = 0.3f;
        public float baseHeight = 0f;
    }
}