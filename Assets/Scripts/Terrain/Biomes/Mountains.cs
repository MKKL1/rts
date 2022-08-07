using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Mountains : Biome
    {
        
        private FastNoiseLite terrainNoise;
        private static GeneratorSettings genSettingsInstance;
        public Mountains(int seed) : base("Mountains")
        {
            genSettingsInstance = GeneratorSettings.instance;
            terrainNoise = new FastNoiseLite(seed+3);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            terrainNoise.SetFractalLacunarity(3f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(3);
            terrainNoise.SetFractalWeightedStrength(1f);
        }

        public override float GetHeight(float x, float y)
        {
            return Utils.normalizedHeight(terrainNoise.GetNoise(x, y)* 0.6f + genSettingsInstance.plainsHeightAdd+0.3f);
        }
    }
}