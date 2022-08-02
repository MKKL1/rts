using Assets.Scripts.Terrain.Biomes;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Terrain
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

        private BiomesManager biomesManager;
        private Biome plainsBiome;
        private Biome waterBiome;

        private int seed;
        public TerrainGenerator(ref GameGrid gameGrid, int seed = 69)
        {
            this.gameGrid = gameGrid;
            terrainSize = gameGrid.gridSize;
            heightmap = new float[terrainSize.x, terrainSize.y];
            this.seed = seed;

            biomesManager = new BiomesManager(seed);

            plainsBiome = biomesManager.GetBiomeFromBiomeType(BiomesType.PLAINS);
            waterBiome = biomesManager.GetBiomeFromBiomeType(BiomesType.WATER);
        }

        private FastNoiseLite newBiomeNoise(int _seed)
        {
            FastNoiseLite noise = new FastNoiseLite(_seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.003f);

            return noise;
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

            for (int i = 0; i < gameGrid.gridSize.x; i++)
                for (int j = 0; j < gameGrid.gridSize.y; j++)
                {
                    float noiseheight = biomeHeightMap[i, j];
                    BiomesType currentbiome =  biomesManager.GetBiomeTypeFromHeight(noiseheight);

                    if (currentbiome == BiomesType.PLAINS)
                        biomeMapTexture.SetPixel(i, j, Color.green);
                    else if (currentbiome == BiomesType.WATER)
                        biomeMapTexture.SetPixel(i, j, Color.blue);

                    GridElement el = new GridElement();
                    el.biome = currentbiome;
                    gameGrid.grid[i, j] = el;
                        
                }

            
        }

        public void generateTerrain()
        {
            generateBiome();

            float b = (waterBiome.biomeAltitide.max - waterBiome.biomeBlendingValue);
            float a = 1/(plainsBiome.biomeAltitide.min + plainsBiome.biomeBlendingValue - (waterBiome.biomeAltitide.max - waterBiome.biomeBlendingValue));

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
                        heightmap[j, i] = normalizedHeight(biomesManager.GetBiomeFromBiomeType(gameGrid.grid[i, j].biome).GetHeight(i, j));
                }
            biomeMapTexture.Apply();
        }

        private float normalizedHeight(float height)
        {
            return (height + 1) * 0.5f;
        }
    }
}