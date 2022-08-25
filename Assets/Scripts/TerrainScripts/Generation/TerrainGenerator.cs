using Assets.Scripts.DebugTools;
using Assets.Scripts.TerrainScripts.BiomeBlending;
using Assets.Scripts.TerrainScripts.Biomes;
using Assets.Scripts.TerrainScripts.Details;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation
{
    public class TerrainGenerator
    {
        public BlendingMethod blendingMethod = BlendingMethod.LerpBlending;

        private TerrainGrid terrainGrid;
        //private MainGrid mainGrid;
        private BiomesManager biomesManager;
        private TerrainGenSettings generatorData;
        private BiomeWeightManager[,] biomeWeightManagers;

        private readonly ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 8 };

        private int seed;
        public TerrainGenerator(int sizeX, int sizeY, float cellSize, TerrainGenSettings generatorData, int seed = 69)
        {
            terrainGrid = new TerrainGrid(sizeX, sizeY, cellSize, cellSize);

            this.generatorData = generatorData;
            this.seed = seed;
            heightmap = new float[terrainGrid.gridSize.x, terrainGrid.gridSize.y];

            biomesManager = new BiomesManager(seed);
        }

        public MainGrid CreateMainGrid(int sizeX, int sizeY)
        {
            mainGrid = new MainGrid(sizeX, sizeY, terrainGrid.GetTerrainWorldSize());
            return mainGrid;
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
            DebugTexture biomeAltTexture = new DebugTexture(terrainGrid.gridSize.x, terrainGrid.gridSize.y);
            float[,] biomeHeightMap = new float[x,y];

            FastNoiseLite noise1 = NewBiomeNoise(seed);
            FastNoiseLite noise2 = NewBiomeNoise(seed + 1);

            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                biomeHeightMap[x, y] = (noise1.GetNoise(x, y) + noise2.GetNoise(x, y) + 2) * 0.25f;
            });
            biomeAltTexture.SetFromArray(biomeHeightMap);
            biomeAltTexture.SaveToPath("DebugTexture/", "biomeAlt");

            return biomeHeightMap;
        }

        private void GenerateBiome()
        {
            DebugTexture biomeTexture = new DebugTexture(terrainGrid.gridSize.x, terrainGrid.gridSize.y);

            biomeWeightManager = new BiomeWeightManager(biomesManager, terrainGrid.gridSize);
            float[,] biomeHeightMap = GetBiomeAltitudeMap(terrainGrid.gridSize.x, terrainGrid.gridSize.y);

            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                BiomeType currentbiome = biomesManager.GetBiomeType(biomeHeightMap[x, y]);
                biomeWeightManager.SetWeight(currentbiome, x, y);

                terrainGrid.biomeGrid[x, y] = currentbiome;
                biomeTexture.SetPixel(x, y, biomesManager.GetBiome(currentbiome).biomeData.biomeColor);
            });

            biomeTexture.SaveToPath("DebugTexture/", "biome");

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

        private TerrainResourceNode[,] GenerateFeatures()
        {
            TerrainResourceNode[,] resourceMap = new TerrainResourceNode[mainGrid.size.x, mainGrid.size.y];
            walkable = new bool[mainGrid.size.x, mainGrid.size.y];
            DebugTexture walkableTextureMap = new DebugTexture(mainGrid.size.x, mainGrid.size.y);
            DebugTexture resourceTextureMap = new DebugTexture(mainGrid.size.x, mainGrid.size.y);
            ResourceGenerator resourceGenerator = new ResourceGenerator(generatorData, biomesManager, terrainGrid, seed);
            for(int x = 1; x < mainGrid.size.x-1; x++)
                for (int y = 1; y < mainGrid.size.y-1 ; y++)
                {
                    bool walkableNode = true;
                    Vector2 worldPosition = mainGrid.GetWorldPosition(x, y);
                    BiomeType biomeType = terrainGrid.GetCellAtWorldPos(worldPosition);
                    Biome currentBiome = biomesManager.GetBiome(biomeType);
                    if (currentBiome.biomeData.resources)
                    {
                        float percentageSpawn = biomeWeightManager.GetWeight(biomeType, x, y);
                        if(percentageSpawn == 1f || percentageSpawn > Random.value)
                        {
                            TerrainResourceNode resNode = resourceGenerator.GetResourceID(x, y);
                            resourceMap[x, y] = resNode;
                            if(resNode.prefabsList != ResourcePrefabsList.NONE) walkableNode = false;
                        }
                    }
                    if(walkableNode && !currentBiome.biomeData.walkable) walkableNode = false;
                    walkable[x, y] = walkableNode;
                }

            walkableTextureMap.SetFromArray(walkable);
            walkableTextureMap.SaveToPath("DebugTexture/", "walkable");

            resourceTextureMap.SetFromArray(resourceMap);
            resourceTextureMap.SaveToPath("DebugTexture/", "resource");
            return resourceMap;
        }

        private byte[,] GenerateTerrain()
        {
            DebugTexture biomeBlendingTexture = new DebugTexture(terrainGrid.gridSize.x, terrainGrid.gridSize.y);
            DebugTexture height = new DebugTexture(terrainGrid.gridSize.x, terrainGrid.gridSize.y);
            byte[,] heightMap = new byte[terrainGrid.gridSize.x, terrainGrid.gridSize.y];
            IterateChunks(chunksize, terrainGrid.gridSize.x, terrainGrid.gridSize.y, (x, y) =>
            {
                float heightSum = 0f;
                foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(x, y))
                {
                    if (entry.Value != 0f)
                        heightSum += biomesManager.GetBiome(entry.Key).GetHeight(x, y) * entry.Value;
                    if (entry.Key == BiomeType.WATER) biomeBlendingTexture.SetPixel(x, y, entry.Value);
                }
                heightMap[y, x] = (byte)(Mathf.Clamp(heightSum * 255, 0, 255));
                height.SetPixel(x, y, heightSum);
            });
            biomeBlendingTexture.SaveToPath("DebugTexture/", "waterbiome");
            height.SaveToPath("DebugTexture/", "height");
            return heightMap;
        }

        //TODO find big enough area for each player, if not generate area
        //      ensure that there is at least single path between all player bases
        public List<Vector3> GenerateStartingAreas(int playerCount)
        {
            return null;
        }

        public TerrainGeneratorResult Generate()
        {
            GenerateBiome();
            TerrainGeneratorResult terrainGeneratorMsg = new TerrainGeneratorResult()
            {
                heightMap = GenerateTerrain(),
                resourceMap = GenerateFeatures(),
                walkableMap = walkable,
                biomeGrid = terrainGrid.biomeGrid,
                mainGridSize = mainGrid.size,
                terrainSize = terrainGrid.GetTerrainWorldSize(),
                terrainGridSize = terrainGrid.gridSize
            };
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