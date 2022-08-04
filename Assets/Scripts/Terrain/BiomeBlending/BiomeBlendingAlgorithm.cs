using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain.BiomeBlending
{
    public interface BiomeBlendingAlgorithm
    {
        public void blendBiomes(ref BiomeWeightManager biomeWeightManager);
    }
}