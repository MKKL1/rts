using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Water : Biome
    {
        public static RangeAttribute biomeAltitide;
        public static float biomeBlendingValue;
        private FastNoiseLite terrainNoise;

        public static void init()
        {
            biomeAltitide = new RangeAttribute(0f, GeneratorSettings.instance.waterTreshold);
            biomeBlendingValue = GeneratorSettings.instance.waterBlending;
        }
        public Water(int seed)
        {
            terrainNoise = new FastNoiseLite(seed + 2);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.002f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(5f);
            terrainNoise.SetFractalOctaves(2);

            biomeName = "Water";
            buildable = false;
            walkable = false;
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y)*0.15f-0.9f;
        }
    }
}