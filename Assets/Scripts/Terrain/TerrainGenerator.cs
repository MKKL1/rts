using System.Collections;
using UnityEngine;

namespace Assets
{
    //TODO remove hard coded biome blending
    public class TerrainGenerator
    {
        public float[,] heightmap;
        //public Biome[,] biomeMap;
        public Texture2D biomeMapTexture;
        

        private Vector2Int terrainSize;
        private GameGrid gameGrid;
        private float[,] biomeHeightMap;

        private Biome plainsBiome;
        private Biome waterBiome;

        private int seed;
        public TerrainGenerator(ref GameGrid gameGrid, int seed = 69)
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
            noise.SetFrequency(0.003f);

            return noise;
        }

        private bool inRange(RangeAttribute range, float value)
        {
            return value >= range.min && value <= range.max;
        }

        private void generateBiome()
        {
            //gameGrid.grid = new GridElement[gameGrid.gridSize.x, gameGrid.gridSize.y];
            biomeHeightMap = new float[terrainSize.x, terrainSize.y];

            biomeMapTexture = new Texture2D(gameGrid.gridSize.x, gameGrid.gridSize.y, TextureFormat.ARGB32, false);

            FastNoiseLite noise1 = newBiomeNoise(seed);
            FastNoiseLite noise2 = newBiomeNoise(seed + 1);

            for (int i = 0; i < terrainSize.x; i++)
                for (int j = 0; j < terrainSize.y; j++)
                {
                    biomeHeightMap[i, j] = (noise1.GetNoise(i, j) + noise2.GetNoise(i, j) + 2) * 0.25f;
                }

            Vector2Int startMiddlePoint = gameGrid.chunkGridSize / 2;

            for (int i = 0; i < gameGrid.gridSize.x; i++)
                for (int j = 0; j < gameGrid.gridSize.y; j++)
                {
                    float noiseheight = biomeHeightMap[startMiddlePoint.x + (i*gameGrid.chunkGridSize.x), startMiddlePoint.y + (j * gameGrid.chunkGridSize.y)];
                    Biome currentbiome = null;

                    

                    if (inRange(Plains.biomeAltitide, noiseheight))
                    {
                        currentbiome = new Plains(seed);
                        biomeMapTexture.SetPixel(i, j, Color.green);
                    }
                    else if (inRange(Water.biomeAltitide, noiseheight))
                    {
                        currentbiome = new Water(seed);
                        biomeMapTexture.SetPixel(i, j, Color.blue);
                    }

                    if (currentbiome != null)
                    {
                        GridElement el = new GridElement();
                        el.biome = currentbiome;
                        gameGrid.grid[i, j] = el;
                    }
                        
                }

            biomeMapTexture.Apply();
        }

        public void generateTerrain()
        {
            generateBiome();

            float b = (Water.biomeAltitide.max - Water.biomeBlendingValue);
            float a = 1/(Plains.biomeAltitide.min + Plains.biomeBlendingValue - (Water.biomeAltitide.max - Water.biomeBlendingValue));

            for (int i = 0; i < gameGrid.gridSize.x; i++)
                for(int j = 0; j < gameGrid.gridSize.y; j++)
                {
                    for(int xInChunk = 0; xInChunk < gameGrid.chunkGridSize.x; xInChunk++)
                        for(int yInChunk = 0; yInChunk < gameGrid.chunkGridSize.y; yInChunk++)
                        {
                            int x = (i * gameGrid.chunkGridSize.x) + xInChunk;
                            int y = (j * gameGrid.chunkGridSize.y) + yInChunk;
                            if (biomeHeightMap[x,y] >= Water.biomeAltitide.max-Water.biomeBlendingValue && biomeHeightMap[x, y] <= Plains.biomeAltitide.min+ Plains.biomeBlendingValue)
                                //Reversed x and y to match minimap
                                heightmap[y, x] = normalizedHeight(Mathf.Lerp(waterBiome.GetHeight(x, y), plainsBiome.GetHeight(x, y), (biomeHeightMap[x, y] - b) *a));
                            else
                                heightmap[y, x] = normalizedHeight(gameGrid.grid[i, j].biome.GetHeight(x, y));
                        }
                }
        }

        private float normalizedHeight(float height)
        {
            return (height + 1) * 0.5f;
        }
    }
}