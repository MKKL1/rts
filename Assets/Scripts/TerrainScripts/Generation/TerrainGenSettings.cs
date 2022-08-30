using Assets.Scripts.TerrainScripts.Biomes;
using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    [CreateAssetMenu]
    public class TerrainGenSettings : ScriptableObject
    {
        public float biomeAltitudeFrequency = 0.006f;
        public BiomeData waterData;
        public BiomeData beachData;
        public BiomeData plainsData;
        public BiomeData mountainsData;
        public ResourceIDManager resourceIDManager;
    }
}