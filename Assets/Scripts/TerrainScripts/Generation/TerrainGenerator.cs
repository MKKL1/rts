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

            IterateChunks((x, y) =>
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

            IterateChunks((x, y) =>
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

        public void GenerateFeatures(ref MainGrid mainGrid)
        {
            FeaturesGenerator featuresGenerator = new FeaturesGenerator(generatorData, seed);
            IterateChunks((x, y) =>
            {
                float heightSum = 0f;
                foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(x, y))
                {
                    if (entry.Value != 0f)
                        heightSum += biomesManager.GetBiome(entry.Key).GetHeight(x, y) * entry.Value;
                }
                heightmap[y, x] = heightSum;
            });
        }

        public void GenerateTerrain()
        {
            //TODO fix paraller for using not async methods
            GenerateBiome();

            //Copied from GenerateBiome()
            IterateChunks((x, y) =>
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
        private void IterateChunks(System.Action<int, int> action)
        {
            //Spliting terrain into chunks to start task with more data to process
            int chunksxcount = Mathf.CeilToInt((float)terrainGrid.gridSize.x / chunksize);
            int chunksycount = Mathf.CeilToInt((float)terrainGrid.gridSize.y / chunksize);

            Parallel.For(0, chunksxcount * chunksycount, parallelOptions, k =>
            {
                int i = k / chunksxcount;
                int j = k % chunksycount;
                for (int x = i * chunksize; x < (i + 1) * chunksize && x < terrainGrid.gridSize.x; x++)
                    for (int y = j * chunksize; y < (j + 1) * chunksize && y < terrainGrid.gridSize.y; y++)
                    {
                        action.Invoke(x, y);
                    }
            });
        }

        
    }
}