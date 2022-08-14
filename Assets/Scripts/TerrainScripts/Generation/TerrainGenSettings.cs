using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    [CreateAssetMenu]
    public class TerrainGenSettings : ScriptableObject
    {
        public float biomeAltitudeFrequency = 0.006f;
        public TerrainResourceIDManager terrainResourceIDManager;
    }
}