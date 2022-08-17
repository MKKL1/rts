using Assets.Scripts.TerrainScripts.BiomeBlending;
using Assets.Scripts.TerrainScripts.Biomes;
using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public struct TerrainGeneratorMsg
    {
        public byte[,] heightMap;
        public TerrainResourceNode[,] resourceMap;
    }
    public class TerrainGenerator
    {
        private readonly int chunksize = 128;

        public float[,] heightmap { get; }
        public BlendingMethod blendingMethod = BlendingMethod.LerpBlending;
        public Transform detailTransform = null;     

        private Vector2Int terrainSize;
        private TerrainGrid terrainGrid;
        private MainGrid mainGrid = GameMain.instance.mainGrid;
        private BiomesManager biomesManager;
        private BiomeWeightManager biomeWeightManager;
        private TerrainGenSettings generatorData;

        private TerrainGeneratorMsg terrainGeneratorMsg = new TerrainGeneratorMsg();


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
            noise.SetFrequency(generatorData.biomeAltitudeFrequency);

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

            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                BiomeType currentbiome = biomesManager.GetBiomeType(biomeHeightMap[x, y]);
                biomeWeightManager.SetWeight(currentbiome, x, y);

                terrainGrid.grid[x, y].biome = currentbiome;
            });

            //Blending
            BiomeBlendingAlgorithm blendingAlgorithm = null;
            switch (blendingMethod)
            {
                case BlendingMethod.LerpBlending:
                    blendingAlgorithm = new LerpBlending(biomesManager, biomeHeightMap);
                    break;
            }

            if(blendingAlgorithm == null)
                throw new System.Exception("Blending algorithm was not found");

            blendingAlgorithm.blendBiomes(ref biomeWeightManager);
        }

        private void GenerateFeatures()
        {
            terrainGeneratorMsg.resourceMap = new TerrainResourceNode[mainGrid.size.x, mainGrid.size.y];
            ResourceGenerator resourceGenerator = new ResourceGenerator(generatorData, biomesManager, terrainGrid, seed);
            for(int x = 1; x < mainGrid.size.x-1; x++)
                for (int y = 1; y < mainGrid.size.y-1 ; y++)
                {
                    Vector2 worldPosition = mainGrid.GetWorldPosition(x, y);
                    BiomeType biomeType = terrainGrid.GetCellAtWorldPos(worldPosition).biome;
                    if (biomesManager.GetBiome(biomeType).biomeData.resources)
                    {
                        float percentageSpawn = biomeWeightManager.GetWeight(biomeType, x, y);
                        if(percentageSpawn == 1f || percentageSpawn > Random.value)
                            terrainGeneratorMsg.resourceMap[x, y] = resourceGenerator.GetResourceID(x, y);
                    }
                }
        }

        public void GenerateTerrain()
        {
            terrainGeneratorMsg.heightMap = new byte[terrainGrid.gridSize.x, terrainGrid.gridSize.y];
            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                float heightSum = 0f;
                foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(x, y))
                {
                    if (entry.Value != 0f)
                        heightSum += biomesManager.GetBiome(entry.Key).GetHeight(x, y) * entry.Value;
                }
                terrainGeneratorMsg.heightMap[y, x] = (byte)(heightSum * 255);
            });
        }

        public TerrainGeneratorMsg Generate()
        {
            GenerateBiome();
            GenerateTerrain();
            GenerateFeatures();
            return terrainGeneratorMsg;
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