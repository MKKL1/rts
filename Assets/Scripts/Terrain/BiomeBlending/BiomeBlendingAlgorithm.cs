using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain.BiomeBlending
{
    public enum BlendingMethod
    {
        LerpBlending,
        GuassianBlur
    }
    public interface BiomeBlendingAlgorithm
    {
        public void blendBiomes(ref BiomeWeightManager biomeWeightManager);
    }
}