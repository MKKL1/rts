using Assets.Scripts.DebugTools;
using Assets.Scripts.TerrainScripts.BiomeBlending;
using Assets.Scripts.TerrainScripts.Biomes;
using Assets.Scripts.TerrainScripts.Details;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation
{
    public class TerrainGenerator
    {
        public TerrainGrid terrainGrid;
        //private MainGrid mainGrid;
        private BiomesManager biomesManager;
        private TerrainGenSettings generatorData;
        private BiomeWeightManager[,] biomeWeightManagers;
        private System.Random rnd;

        //private readonly ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 8 };

        private int seed;
        public TerrainGenerator(int sizeX, int sizeY, float cellSize, TerrainGenSettings generatorData, int seed = 69)
        {
            terrainGrid = new TerrainGrid(sizeX, sizeY, cellSize, cellSize);

            this.generatorData = generatorData;
            this.seed = seed;
            rnd = new System.Random(seed);
            InitBiomeManager();
        }

        private void InitBiomeManager()
        {
            biomesManager = new BiomesManager();
            biomesManager.addBiome(new Plains(seed));
            biomesManager.addBiome(new Water(seed));
            biomesManager.addBiome(new Beach(seed));
            biomesManager.addBiome(new Mountains(seed));
            biomesManager.initBiomes();
        }

        public MainGrid CreateMainGrid(int sizeX, int sizeY)
        {
            MainGrid mainGrid = new MainGrid(sizeX, sizeY, terrainGrid.worldGridSize);
            mainGrid.IterateChunks(new Action<int, int>((x,y) =>
            {
                Vector2Int chunkSize = mainGrid.GetChunkSizeAt(x, y);
                mainGrid.chunks[x, y] = new MainGridChunk((ushort)chunkSize.x, (ushort)chunkSize.y);
            }));
            return mainGrid;
        }

        private FastNoiseLite NewBiomeNoise(int _seed)
        {
            FastNoiseLite noise = new FastNoiseLite(_seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(generatorData.biomeAltitudeFrequency);

            return noise;
        }

        public void Generate()
        {
            terrainGrid.IterateChunks(new Action<int, int> ((xChunk, yChunk) =>
            {
                Vector2Int chunkSize = terrainGrid.GetChunkSizeAt(xChunk, yChunk);
                terrainGrid.chunks[xChunk, yChunk] = new TerrainChunk((ushort)chunkSize.x, (ushort)chunkSize.y);
            }));

            //TODO generate biome altitude map in new class
            FastNoiseLite noise1 = NewBiomeNoise(seed);
            FastNoiseLite noise2 = NewBiomeNoise(seed + 1);

            biomeWeightManagers = new BiomeWeightManager[terrainGrid.chunkArrayLength.x, terrainGrid.chunkArrayLength.y];
            Task[] tasks = terrainGrid.IterateChunksAsync(new Action<int, int>((xChunk, yChunk) =>
            {
                TerrainChunk currentChunk = terrainGrid.chunks[xChunk, yChunk];
                float[,] biomeHeightMap = new float[currentChunk.chunkSizeX, currentChunk.chunkSizeY];

                BiomeWeightManager biomeWeightManager = biomeWeightManagers[xChunk, yChunk];
                biomeWeightManager = new BiomeWeightManager(biomesManager, currentChunk.chunkSizeX, currentChunk.chunkSizeY);


                //Generate biomes
                terrainGrid.IterateInChunk(currentChunk, new Action<int, int>((xInChunk, yInChunk) =>
                {
                    int xAtGrid = (xChunk * terrainGrid.chunkSize) + xInChunk;
                    int yAtGrid = (yChunk * terrainGrid.chunkSize) + yInChunk;

                    float biomeHeight = (noise1.GetNoise(xAtGrid, yAtGrid) + noise2.GetNoise(xAtGrid, yAtGrid) + 2) * 0.25f;
                    biomeHeightMap[xInChunk, yInChunk] = biomeHeight;
                    BiomeType biomeType = biomesManager.GetBiomeType(biomeHeight);
                    currentChunk.biomeGrid[xInChunk, yInChunk] = biomeType;
                    biomeWeightManager.SetWeight(biomeType, xInChunk, yInChunk);
                }));

                //Blend biomes
                LerpBlending lerpBlending = new LerpBlending(biomesManager);
                lerpBlending.blendBiomes(ref biomeWeightManager, biomeHeightMap);

                //Generate terrain heightmap
                terrainGrid.IterateInChunk(currentChunk, new Action<int, int>((xInChunk, yInChunk) =>
                {
                    int xAtGrid = (xChunk * terrainGrid.chunkSize) + xInChunk;
                    int yAtGrid = (yChunk * terrainGrid.chunkSize) + yInChunk;

                    float heightSum = 0f;
                    foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(xInChunk, yInChunk))
                    {
                        if (entry.Value != 0f)
                            heightSum += biomesManager.GetBiome(entry.Key).GetHeight(xAtGrid, yAtGrid) * entry.Value;
                    }
                    currentChunk.heightMap[xInChunk, yInChunk] = heightSum;
                }));
                biomeWeightManagers[xChunk, yChunk] = biomeWeightManager;
                Debug.Log($"[Generation] Chunk({xChunk},{yChunk}) heightmap generated");
            }));
            Task.WaitAll(tasks);
            Debug.Log($"[Generation] All chunks heightmaps generated");
        }


        public void GenerateFeatures(MainGrid mainGrid)
        {
            ResourceGenerator resourceGenerator = new ResourceGenerator(mainGrid.gridDataSize, generatorData, seed);
            Task[] tasks = mainGrid.IterateChunksAsync(new Action<int, int>((xChunk, yChunk) =>
            {
                MainGridChunk currentChunk = mainGrid.chunks[xChunk, yChunk];
                BiomeWeightManager biomeWeightManager = biomeWeightManagers[xChunk, yChunk];
                mainGrid.IterateInChunk(currentChunk, new Action<int, int>((xInChunk, yInChunk) =>
                {
                    
                    int xAtGrid = (xChunk * mainGrid.chunkSize) + xInChunk;
                    int yAtGrid = (yChunk * mainGrid.chunkSize) + yInChunk;

                    bool walkableNode = false;
                    //TODO better way to not spawn resources at edges of terrain
                    if (!(xAtGrid == 0 || xAtGrid == mainGrid.worldGridSize.x-1 
                        ||yAtGrid == 0 || yAtGrid == mainGrid.worldGridSize.y - 1)) 
                    {
                        walkableNode = true;
                        Vector2 worldPosition = mainGrid.GetWorldPosition(xAtGrid, yAtGrid);
                        BiomeType biomeType = terrainGrid.GetBiomeAtWorldPos(worldPosition);
                        Biome currentBiome = biomesManager.GetBiome(biomeType);
                        if (currentBiome.biomeData.resources)
                        {
                            //TODO xInChunk is from terrainGrid not mainGrid
                            int percentageSpawn = (int)(biomeWeightManager.GetWeight(biomeType, xInChunk, yInChunk) * 100);
                            if (percentageSpawn == 100 || rnd.Next(0, 100) < percentageSpawn)
                            {
                                TerrainResourceNode resNode = resourceGenerator.GetResourceID(xAtGrid, yAtGrid);
                                currentChunk.resourceMap[xInChunk, yInChunk] = resNode;
                                if (resNode.prefabsList != ResourcePrefabsList.NONE) walkableNode = false;
                            }
                        }

                        if (walkableNode && !currentBiome.biomeData.walkable)
                            walkableNode = false;
                    }
                    currentChunk.walkableMap[xInChunk, yInChunk] = walkableNode;
                }));
            }));
            Task.WaitAll(tasks);
        }

        //TODO find big enough area for each player, if not generate area
        //      ensure that there is at least single path between all player bases
        public List<Vector3> GenerateStartingAreas(int playerCount)
        {
            return null;
        }

        public void GenerateAll(MainGrid mainGrid)
        {
            Generate();
            GenerateFeatures(mainGrid);
        }
    }
}