using System.Collections;
using UnityEngine;

namespace Assets
{
    //TODO improve generation, generate more flat terrains while keeping hills 
    public class TerrainGenerator
    {
        private int xSize, ySize;
        public float[,] heightmap;
        FastNoiseLite[] noises;
        public TerrainGenerator(int xSize, int ySize, int seed = 69)
        {
            
            this.xSize = xSize;
            this.ySize = ySize;
            heightmap = new float[xSize, ySize];
            noises = new FastNoiseLite[3];

            
            
            

            noises[0] = new FastNoiseLite(seed);
            noises[0].SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noises[0].SetFrequency(0.002f);
            noises[0].SetFractalType(FastNoiseLite.FractalType.FBm);
            noises[0].SetFractalLacunarity(2f);
            noises[0].SetFractalGain(0.5f);
            noises[0].SetFractalOctaves(4);


            //noises[1] = new FastNoiseLite(seed + 1);
            //noises[1].SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //noises[1].SetFrequency(basefrequency * lacurnity * lacurnity);
        }

        public void generateTerrain()
        {
            float minheight = float.MaxValue, maxheight = float.MinValue;
            for(int i = 0; i < xSize; i++)
                for(int j = 0; j < ySize; j++)
                {
                    float height = (noises[0].GetNoise(i, j) + 1f) * 0.5f;
                    if (height < minheight) minheight = height;
                    else if (height > maxheight) maxheight = height;
                    heightmap[i, j] = height;
                }
            float heightDifference = maxheight - minheight;
            
        }
    }
}