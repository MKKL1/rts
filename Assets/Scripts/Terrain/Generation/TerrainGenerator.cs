using Assets.Scripts.Terrain.BiomeBlending;
using Assets.Scripts.Terrain.Biomes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    //TODO remove hard coded biome blending
    public class TerrainGenerator
    {
        public float[,] heightmap;
        //public Biome[,] biomeMap;
        public Texture2D biomeMapTexture;
        public Texture2D plainsMapTexture;
        

        private Vector2Int terrainSize;
        private GameGrid gameGrid;

        private BiomesManager biomesManager;
        private BiomeWeightManager biomeWeightManager;
        private Biome plainsBiome;
        private Biome waterBiome;

        private int seed;
        public TerrainGenerator(ref GameGrid gameGrid, int seed = 69)
        {
            this.gameGrid = gameGrid;
            this.seed = seed;
            terrainSize = gameGrid.gridSize;
            heightmap = new float[terrainSize.x, terrainSize.y];

            biomesManager = new BiomesManager(seed);

            plainsBiome = biomesManager.GetBiome(BiomeType.PLAINS);
            waterBiome = biomesManager.GetBiome(BiomeType.WATER);
        }

        private FastNoiseLite NewBiomeNoise(int _seed)
        {
            FastNoiseLite noise = new FastNoiseLite(_seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.003f);

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
            biomeWeightManager = new BiomeWeightManager(biomesManager, gameGrid.gridSize);

            float[,] biomeHeightMap = GetBiomeAltitudeMap(gameGrid.gridSize.x, gameGrid.gridSize.y);
            biomeMapTexture = new Texture2D(gameGrid.gridSize.x, gameGrid.gridSize.y, TextureFormat.ARGB32, false);

            for (int i = 0; i < gameGrid.gridSize.x; i++)
                for (int j = 0; j < gameGrid.gridSize.y; j++)
                {
                    float noiseheight = biomeHeightMap[i, j];
                    BiomeType currentbiome =  biomesManager.GetBiomeType(noiseheight);
                    biomeWeightManager.SetWeight(currentbiome, i, j);

                    biomeMapTexture.SetPixel(i, j, biomesManager.GetBiomeColor(currentbiome));
                    gameGrid.grid[i, j].biome = currentbiome;
                }


            BiomeBlendingAlgorithm blendingAlgorithm = new LerpBlending(biomesManager, biomeHeightMap);
            blendingAlgorithm.blendBiomes(ref biomeWeightManager);
            //BiomeGuassianBlur guassianBlur = new BiomeGuassianBlur(ref biomeWeightManager);
            //guassianBlur.Process(2);
        }

        public void GenerateTerrain()
        {
            GenerateBiome();
            plainsMapTexture = new Texture2D(gameGrid.gridSize.x, gameGrid.gridSize.y, TextureFormat.ARGB32, false);
            //float b = (waterBiome.biomeAltitide.max - waterBiome.biomeBlendingValue);
            //float a = 1/(plainsBiome.biomeAltitide.min + plainsBiome.biomeBlendingValue - (waterBiome.biomeAltitide.max - waterBiome.biomeBlendingValue));

            for (int i = 0; i < gameGrid.gridSize.x; i++)
                for(int j = 0; j < gameGrid.gridSize.y; j++)
                {

                    //if (biomeHeightMap[i, j] >= waterBiome.biomeAltitide.max - waterBiome.biomeBlendingValue && biomeHeightMap[i,j] <= plainsBiome.biomeAltitide.min + plainsBiome.biomeBlendingValue)
                    //{
                    //    //Reversed x and y to match minimap
                    //    heightmap[j, i] = normalizedHeight(Mathf.SmoothStep(waterBiome.GetHeight(i, j), plainsBiome.GetHeight(i, j), (biomeHeightMap[i, j] - b) * a));
                    //    biomeMapTexture.SetPixel(i, j, Color.red);
                    //}
                    //else
                    float heightSum = 0f;
                    foreach (KeyValuePair<BiomeType, float> entry in biomeWeightManager.GetWeight(i, j))
                    {
                        if(entry.Value != 0f)
                            heightSum += biomesManager.GetBiome(entry.Key).GetHeight(i, j) * entry.Value;

                        if(entry.Key == BiomeType.PLAINS)
                        {
                            byte c = (byte)(255 * entry.Value);
                            plainsMapTexture.SetPixel(i, j, new Color(c, c, c));
                        }
                            
                    }
                    heightmap[j, i] = normalizedHeight(heightSum);
                }
            biomeMapTexture.Apply();
            plainsMapTexture.Apply();
        }

        private float normalizedHeight(float height)
        {
            return (height + 1) * 0.5f;
        }
    }
}