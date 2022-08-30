using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Biomes
{
    public class Water : Biome
    {
        private FastNoiseLite terrainNoise;
        public Water(int seed, BiomeData biomeData) : base(biomeData)
        {
            terrainNoise = new FastNoiseLite(seed + 2);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.002f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(5f);
            terrainNoise.SetFractalOctaves(2);
        }

        public override float GetHeight(float x, float y)
        {
            return Utils.normalizedHeight(terrainNoise.GetNoise(x, y)*0.15f-0.9f);
        }
    }
}