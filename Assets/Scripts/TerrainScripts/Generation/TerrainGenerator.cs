﻿using Assets.Scripts.TerrainScripts.BiomeBlending;
using Assets.Scripts.TerrainScripts.Biomes;
using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public struct TerrainGeneratorData
    {
        public byte[,] heightMap;
        public TerrainResource[,] terrainResourceMap;
    }
    public class TerrainGenerator
    {
        private readonly int chunksize = 128;

        public float[,] heightmap { get; }
        public BlendingMethod blendingMethod = BlendingMethod.LerpBlending;
        public float biomeAltitudeNoiseFrequency = 0.003f;

        public Texture2D biomeMapTexture;
        

        private Vector2Int terrainSize;
        private TerrainGrid terrainGrid;
        private MainGrid mainGrid = GameMain.instance.mainGrid;
        private BiomesManager biomesManager;
        private BiomeWeightManager biomeWeightManager;
        private TerrainGenSettings generatorData;
        

        private readonly ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 8 };

        private int seed;
        public TerrainGenerator(ref TerrainGrid terrainGrid, TerrainGenSettings generatorData, int seed = 69)
        {
            this.terrainGrid = terrainGrid;
            this.generatorData = generatorData;
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

            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                biomeHeightMap[x, y] = (noise1.GetNoise(x, y) + noise2.GetNoise(x, y) + 2) * 0.25f;
            });

            return biomeHeightMap;
        }
        private void GenerateBiome()
        {
            biomeWeightManager = new BiomeWeightManager(biomesManager, terrainGrid.gridSize);

            float[,] biomeHeightMap = GetBiomeAltitudeMap(terrainGrid.gridSize.x, terrainGrid.gridSize.y);
            biomeMapTexture = new Texture2D(terrainGrid.gridSize.x, terrainGrid.gridSize.y, TextureFormat.ARGB32, false);

            Color[,] colorMap = new Color[terrainGrid.gridSize.x, terrainGrid.gridSize.y];

            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                BiomeType currentbiome = biomesManager.GetBiomeType(biomeHeightMap[x, y]);
                biomeWeightManager.SetWeight(currentbiome, x, y);

                colorMap[x, y] = biomesManager.GetBiomeColor(currentbiome);
                terrainGrid.grid[x, y].biome = currentbiome;
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
        }

        public void GenerateFeatures(Terrain terrain)
        {
            FeaturesGenerator featuresGenerator = new FeaturesGenerator(generatorData, seed);
            for(int x = 1; x < mainGrid.size.x-1; x++)
                for (int y = 1; y < mainGrid.size.y-1; y++)
                {
                    GameObject tmpObj = featuresGenerator.GetFeature(x, y);
                    if (tmpObj != null)
                    {
                        Vector2 featurePos2 = Utils.RandomMove(mainGrid.GetScenePosition(x, y), mainGrid.cellSize.x * 0.5f, mainGrid.cellSize.y * 0.5f);
                        
                        Vector3 featurePos = new Vector3(featurePos2.x, terrain.SampleHeight(new Vector3(featurePos2.x, 0, featurePos2.y)), featurePos2.y);
                        GameObject featureObject = Object.Instantiate(tmpObj, featurePos, Quaternion.Euler(0, Random.Range(0, 360), 0));
                        TerrainResource terrainResource = featureObject.GetComponent<TerrainResource>();
                        terrainResource.gridPosX = (short)x;
                        terrainResource.gridPosY = (short)y;

                        mainGrid.terrainResourceMap[x, y] = terrainResource;
                    }
                }
        }

        public void GenerateTerrain()
        {
            //TODO fix paraller for using not async methods
            GenerateBiome();

            //Copied from GenerateBiome()
            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                float heightSum = 0f;
                foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(x, y))
                {
                    if (entry.Value != 0f)
                        heightSum += biomesManager.GetBiome(entry.Key).GetHeight(x, y) * entry.Value;
                }
                heightmap[y, x] = heightSum;
            });
                        
            biomeMapTexture.Apply();
        }

        /// <summary>
        /// Used to iterate over every terrain grid cell using asynchronous action
        /// </summary>
        /// <param name="action"></param>
        private void IterateChunks(int _chunkSize, int sizeX, int sizeY, System.Action<int, int> action)
        {
            //Spliting terrain into chunks to start task with more data to process
            int chunksxcount = Mathf.CeilToInt((float)sizeX / _chunkSize);
            int chunksycount = Mathf.CeilToInt((float)sizeY / _chunkSize);

            Parallel.For(0, chunksxcount * chunksycount, parallelOptions, k =>
            {
                int i = k / chunksxcount;
                int j = k % chunksycount;
                for (int x = i * _chunkSize; x < (i + 1) * _chunkSize && x < sizeX; x++)
                    for (int y = j * _chunkSize; y < (j + 1) * _chunkSize && y < sizeY; y++)
                    {
                        action.Invoke(x, y);
                    }
            });
        }

        
    }
}