using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Beach : Biome
    {
        private FastNoiseLite terrainNoise;
        public Beach(int seed)
        {
            biomeAltitide = new RangeAttribute(0.35f, 0.43f);

            terrainNoise = new FastNoiseLite(seed + 2);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(1);

            biomeName = "Beach";
            type = BiomeType.BEACH;
            biomeColor = Color.yellow;
            blendingValueStart = 0.01f;
            blendingValueEnd = 0.01f;
        }

        public override float GetHeight(float x, float y)
        {
            return -0.62f;
        }
    }
}