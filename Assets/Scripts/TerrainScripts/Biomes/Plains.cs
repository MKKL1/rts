using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Biomes
{
    public class Plains : Biome
    {
        
        private FastNoiseLite terrainNoise;
        public Plains(int seed, BiomeData biomeData) : base(biomeData)
        {
            

            terrainNoise = new FastNoiseLite(seed+1);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(4);
        }

        public override float GetHeight(float x, float y)
        {
            return Utils.normalizedHeight(terrainNoise.GetNoise(x, y)* biomeData.heightMultiplier + biomeData.baseHeight);
        }

        public override GameObject GetFeature(float x, float y)
        {
            return null;
        }
    }
}