using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Plains : Biome
    {
        
        private FastNoiseLite terrainNoise;
        private static GeneratorSettings genSettingsInstance;
        public Plains(int seed)
        {
            genSettingsInstance = GeneratorSettings.instance;
            biomeAltitide = new RangeAttribute(0.43f, 0.8f);

            terrainNoise = new FastNoiseLite(seed+1);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(4);

            biomeName = "Plains";
            type = BiomeType.PLAINS;
            biomeColor = Color.green;
            blendingValueStart = 0.1f;
            blendingValueEnd = 0.08f;
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y)* genSettingsInstance.plainsHeightMultiplier + genSettingsInstance.plainsHeightAdd;
        }
    }
}