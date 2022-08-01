using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class Water : Biome
    {
        public static RangeAttribute biomeAltitide = new RangeAttribute(0f, 0.4f);
        public static float biomeBlendingValue = 0.1f;
        private FastNoiseLite terrainNoise;
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
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y)*0.15f-1f;
        }
    }
}