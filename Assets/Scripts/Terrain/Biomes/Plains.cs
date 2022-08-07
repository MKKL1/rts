﻿using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Terrain.Biomes
{
    public class Plains : Biome
    {
        
        private FastNoiseLite terrainNoise;
        private static GeneratorSettings genSettingsInstance;
        public Plains(int seed):base("Plains")
        {
            
            genSettingsInstance = GeneratorSettings.instance;

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
            return terrainNoise.GetNoise(x, y)* genSettingsInstance.plainsHeightMultiplier + genSettingsInstance.plainsHeightAdd;
        }

        public override GameObject GetFeature(float x, float y)
        {
            return null;
        }
    }
}