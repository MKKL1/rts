using Assets.Scripts.Terrain.BiomeBlending;
using Assets.Scripts.Terrain.Biomes;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    //TODO remove hard coded biome blending
    public class TerrainGenerator
    {
        public float[,] heightmap { get; }
        public BlendingMethod blendingMethod = BlendingMethod.LerpBlending;
        public float biomeAltitudeNoiseFrequency = 0.003f;

        public Texture2D biomeMapTexture;
        

        private Vector2Int terrainSize;
        private TerrainGrid terrainGrid;

        private BiomesManager biomesManager;
        private BiomeWeightManager biomeWeightManager;

        private int seed;
        public TerrainGenerator(ref TerrainGrid terrainGrid, int seed = 69)
        {
            this.terrainGrid = terrainGrid;
            this.seed = seed;
            terrainSize = terrainGrid.gridSize;
            heightmap = new float[terrainSize.x, terrainSize.y];

            biomesManager = new BiomesManager(seed);
        }

        private FastNoiseLite NewBiomeNoise(int _seed)
        {
            FastNoiseLite noise = new FastNoiseLite(_seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(biomeAltitudeNoiseFrequency);

            return noise;
        }

        private float[,] GetBiomeAltitudeMap(int x, int y)
        {
            float[,] biomeHeightMap = new float[x,y];

            FastNoiseLite noise1 = NewBiomeNoise(seed);
            FastNoiseLite noise2 = NewBiomeNoise(seed + 1);

            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    biomeHeightMap[i, j] = (noise1.GetNoise(i, j) + noise2.GetNoise(i, j) + 2) * 0.25f;
                }

            return biomeHeightMap;
        }
        private void GenerateBiome()
        {
            biomeWeightManager = new BiomeWeightManager(biomesManager, terrainGrid.gridSize);

            float[,] biomeHeightMap = GetBiomeAltitudeMap(terrainGrid.gridSize.x, terrainGrid.gridSize.y);
            biomeMapTexture = new Texture2D(terrainGrid.gridSize.x, terrainGrid.gridSize.y, TextureFormat.ARGB32, false);

            Color[,] colorMap = new Color[terrainGrid.gridSize.x, terrainGrid.gridSize.y];

            //Spliting into chunks instead of just making new task for each height map point
            int chunksxsize = Mathf.CeilToInt((float)terrainGrid.gridSize.x / 16f);
            int chunksysize = Mathf.CeilToInt((float)terrainGrid.gridSize.y / 16f);

            Parallel.For(0, chunksxsize, i =>
            {
                Parallel.For(0, chunksysize, j =>
                {
                    for (int x = i * 16; x < (i + 1) * 16 && x < terrainGrid.gridSize.x; x++)
                        for (int y = j * 16; y < (j + 1) * 16 && y < terrainGrid.gridSize.y; y++)
                        {
                            BiomeType currentbiome = biomesManager.GetBiomeType(biomeHeightMap[x, y]);
                            biomeWeightManager.SetWeight(currentbiome, x, y);

                            colorMap[x, y] = biomesManager.GetBiomeColor(currentbiome);
                            terrainGrid.grid[x, y].biome = currentbiome;
                        }
                });
            });

            for (int i = 0; i < terrainGrid.gridSize.x; i++)
                for (int j = 0; j < terrainGrid.gridSize.y; j++)
                {
                    biomeMapTexture.SetPixel(i, j, colorMap[i, j]);
                }


            BiomeBlendingAlgorithm blendingAlgorithm = null;
            switch (blendingMethod)
            {
                case BlendingMethod.LerpBlending:
                    blendingAlgorithm = new LerpBlending(biomesManager, biomeHeightMap);
                    break;
            }
            if(blendingAlgorithm == null)
            {
                throw new System.Exception("Blending algorithm was not found");
            }

            blendingAlgorithm.blendBiomes(ref biomeWeightManager);
            //BiomeGuassianBlur guassianBlur = new BiomeGuassianBlur(ref biomeWeightManager);
            //guassianBlur.Process(2);
        }

        public void GenerateTerrain()
        {
            GenerateBiome();

            //TODO async
            for (int i = 0; i < terrainGrid.gridSize.x; i++)
                for(int j = 0; j < terrainGrid.gridSize.y; j++)
                {
                    float heightSum = 0f;
                    foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(i, j))
                    {
                        if(entry.Value != 0f)
                            heightSum += biomesManager.GetBiome(entry.Key).GetHeight(i, j) * entry.Value;
                    }
                    heightmap[j, i] = normalizedHeight(heightSum);
                }
            biomeMapTexture.Apply();
        }

        private float normalizedHeight(float height)
        {
            return (height + 1) * 0.5f;
        }
    }
}