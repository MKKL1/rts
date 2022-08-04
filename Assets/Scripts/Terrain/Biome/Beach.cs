﻿using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Beach : Biome
    {
        private FastNoiseLite terrainNoise;
        public Beach(int seed)
        {
            biomeAltitide = new RangeAttribute(0.35f, 0.43f);
            biomeBlendingValue = GeneratorSettings.instance.plainsBlending;

            terrainNoise = new FastNoiseLite(seed + 2);
            terrainNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            terrainNoise.SetFrequency(0.005f);
            terrainNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            terrainNoise.SetFractalLacunarity(2f);
            terrainNoise.SetFractalGain(0.5f);
            terrainNoise.SetFractalOctaves(1);

            biomeName = "Beach";
            type = BiomesType.BEACH;
            biomeColor = Color.yellow;
            biomeBlendingValue = 0.05f;
        }

        public override float GetHeight(float x, float y)
        {
            return terrainNoise.GetNoise(x, y)*0.05f-0.6f;
        }
    }
}