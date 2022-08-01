using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Plains : Biome
    {
        public static RangeAttribute biomeAltitide;
        public static float biomeBlendingValue;
        private FastNoiseLite terrainNoise;
        private static GeneratorSettings genSettingsInstance;

        public static void init()
        {
            genSettingsInstance = GeneratorSettings.instance;
            biomeAltitide = new RangeAttribute(genSettingsInstance.waterTreshold, 1f);
            biomeBlendingValue = genSettingsInstance.plainsBlending;
        }
        public Plains(int seed)
        {
            terrainNoise = new FastNoiseLite(seed+1);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(4);

            biomeName = "Plains";
            
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y)* genSettingsInstance.plainsHeightMultiplier + genSettingsInstance.plainsHeightAdd;
        }
    }
}