using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class Plains : Biome
    {
        private FastNoiseLite terrainNoise;
        private FastNoiseLite biomeGenerationNoise;
        public Plains(int seed)
        {
            terrainNoise = new FastNoiseLite(seed+1);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(6);

            biomeName = "Plains";
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y);
        }
    }
}