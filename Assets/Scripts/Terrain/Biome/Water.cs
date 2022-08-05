using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Water : Biome
    {
        private FastNoiseLite terrainNoise;
        public Water(int seed)
        {
            biomeAltitide = new RangeAttribute(0f, 0.35f);

            terrainNoise = new FastNoiseLite(seed + 2);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.002f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(5f);
            terrainNoise.SetFractalOctaves(2);

            biomeName = "Water";
            type = BiomeType.WATER;
            buildable = false;
            walkable = false;
            biomeColor = Color.blue;
            blendingValueStart = 0.08f;
            blendingValueEnd = 0.08f;
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y)*0.15f-0.9f;
        }
    }
}