using System.Collections;
using UnityEngine;

namespace Assets
{
    //TODO improve generation, generate more flat terrains while keeping hills 
    public class TerrainGenerator
    {
        public float[,] heightmap;
        public Biome[,] biomeMap;
        public Texture2D biomeMapTexture;

        private Vector2Int terrainSize;
        private GameGrid gameGrid;
        private float[,] biomeHeightMap;

        private Biome plainsBiome;
        private Biome waterBiome;

        private int seed;
        public TerrainGenerator(GameGrid gameGrid, int seed = 69)
        {
            this.gameGrid = gameGrid;
            terrainSize = gameGrid.gridSize * gameGrid.chunkGridSize;
            heightmap = new float[terrainSize.x, terrainSize.y];
            this.seed = seed;

            plainsBiome = new Plains(seed);
            waterBiome = new Water(seed);
        }

        private FastNoiseLite newBiomeNoise(int _seed)
        {
            FastNoiseLite noise = new FastNoiseLite(_seed);
            noise.SetFrequency(0.03f);

            return noise;
        }

        private void generateBiome()
        {
            biomeMap = new Biome[gameGrid.gridSize.x, gameGrid.gridSize.y];
            biomeHeightMap = new float[gameGrid.gridSize.x, gameGrid.gridSize.y];

            biomeMapTexture = new Texture2D(gameGrid.gridSize.x, gameGrid.gridSize.y, TextureFormat.ARGB32, false);

            FastNoiseLite noise1 = newBiomeNoise(seed);
            FastNoiseLite noise2 = newBiomeNoise(seed + 1);

            for (int i = 0; i < gameGrid.gridSize.x; i++)
                for (int j = 0; j < gameGrid.gridSize.y; j++)
                {
                    float noiseheight = (noise1.GetNoise(i, j) + noise2.GetNoise(i, j) + 2) * 0.25f;
                    biomeHeightMap[i, j] = noiseheight;
                    Biome currentbiome = null;

                    if (noiseheight > 0.5f)
                    {
                        currentbiome = new Plains(seed);
                        biomeMapTexture.SetPixel(i, j, Color.green);
                    }
                    else if (noiseheight <= 0.5f)
                    {
                        currentbiome = new Water(seed);
                        biomeMapTexture.SetPixel(i, j, Color.blue);
                    }

                    if (currentbiome == null) return;

                    biomeMap[i, j] = currentbiome;
                }

            biomeMapTexture.Apply();
        }

        public void generateTerrain()
        {
            generateBiome();

            for(int i = 0; i < gameGrid.gridSize.x; i++)
                for(int j = 0; j < gameGrid.gridSize.y; j++)
                {
                    for(int xInChunk = 0; xInChunk < gameGrid.chunkGridSize.x; xInChunk++)
                        for(int yInChunk = 0; yInChunk < gameGrid.chunkGridSize.y; yInChunk++)
                        {
                            int x = (i * gameGrid.chunkGridSize.x) + xInChunk;
                            int y = (j * gameGrid.chunkGridSize.y) + yInChunk;
                            if (biomeHeightMap[i, j] > 0.45f && biomeHeightMap[i, j] < 0.55f)
                                heightmap[x, y] = normalizedHeight(Mathf.Lerp(waterBiome.GetHeight(x, y), plainsBiome.GetHeight(x, y), (biomeHeightMap[i, j] - 0.45f)*10));
                            else
                                heightmap[x, y] = normalizedHeight(biomeMap[i, j].GetHeight(x, y));
                        }
                }
        }

        private float normalizedHeight(float height)
        {
            return (height + 1) * 0.5f;
        }
    }
}