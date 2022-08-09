﻿using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public class FeaturesGenerator
    {
        private TerrainGenSettings terrainGenSettings;
        private FastNoiseLite treeNoise;
        public FeaturesGenerator(TerrainGenSettings data, int seed)
        {
            terrainGenSettings = data;
            treeNoise = new FastNoiseLite(seed+5);
            treeNoise.SetFrequency(0.01f);
        }

        public GameObject GetFeature(float x, float y)
        {
            if(treeNoise.GetNoise(x, y) > 0.8f)
            {
                return terrainGenSettings.trees[Random.Range(0, terrainGenSettings.trees.Count - 1)];
            }
            return null;
        }
    }
}