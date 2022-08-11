using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation.Noise
{
    class ForestNoiseNode : ICloneable
    {
        public bool isForest = false;
        public byte expensionValue = 0;
        public byte age = 0;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ForestNoise
    {
        private System.Random random;
        public Vector2Int size;
        public int forestAge = 7;
        private ForestNoiseNode[,] noiseGrid;
        private int[,] neighbourNodes = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        private FastNoiseLite noise;

        public ForestNoise(int sizeX, int sizeY, int seed = 69)
        {
            noise = new FastNoiseLite(seed + 1);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(0.01f);

            random = new System.Random(seed);
            size = new Vector2Int(sizeX, sizeY);
            noiseGrid = new ForestNoiseNode[sizeX, sizeY];
        }

        private bool isOccupied(int x, int y)
        {
            return x < 0 || x >= size.x || y < 0 || y >= size.y || noiseGrid[x,y].isForest;
        }



        public void Generate()
        {
            //TODO Remove
            //Debugging
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
            //


            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {

                    ForestNoiseNode node = new ForestNoiseNode();
                    float height = noise.GetNoise(x, y);
                    if (height > 0.5f && random.Next(0, 100) < 5)
                    {
                        
                        node.isForest = true;
                        node.expensionValue = (byte)random.Next(3, 6);
                        
                    } 
                    else if(height > 0.1f && random.Next(0, 100) < 2)
                    {
                        node.isForest = true;
                        node.expensionValue = (byte)random.Next(0, 4);
                    }
                    noiseGrid[x, y] = node;

                }

            //New grid to save changes to while processing old grid
            ForestNoiseNode[,] newNoiseGrid = new ForestNoiseNode[size.x, size.y];

            //Copying to new grid
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    newNoiseGrid[x, y] = (ForestNoiseNode)noiseGrid[x, y].Clone();

            //Using bool to check if any changes happend to save processing time
            bool isAnyExpendable = true;
            for (int i = forestAge; i > 0 && isAnyExpendable; i--)
            {
                isAnyExpendable = false;
                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        ForestNoiseNode node = noiseGrid[x, y];
                        if (node.expensionValue > 0)
                        {
                            isAnyExpendable = true;
                            //Expanding costs 1 point
                            node.expensionValue--;
                            for (int a = 0; a < 4; a++)
                            {
                                //Expanding in each direction (top, bottom, right, left)
                                int eX = x + neighbourNodes[a, 0];
                                int eY = y + neighbourNodes[a, 1];
                                //Checking if there is already forest on node
                                if (isOccupied(eX, eY)) continue;

                                //If node is empty create new child node
                                ForestNoiseNode exnode = (ForestNoiseNode)noiseGrid[eX, eY].Clone();
                                exnode.expensionValue = node.expensionValue;
                                exnode.isForest = true;

                                newNoiseGrid[eX, eY] = exnode;
                            }
                        }
                    }


                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        //Aging all forest nodes
                        if (newNoiseGrid[x, y].isForest) newNoiseGrid[x, y].age++;
                        //Copying to old grid
                        noiseGrid[x, y] = (ForestNoiseNode)newNoiseGrid[x, y].Clone();
                    }
                        

            }


            

            //TODO Remove
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {

                    if (noiseGrid[x, y].isForest)
                    {
                        texture.SetPixel(x, y, new Color(0, noiseGrid[x, y].age * 0.2f, 0));
                    }
                    else texture.SetPixel(x, y, Color.white);
                }

            texture.Apply();

            Debug.Log("Generated");
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = @"C:\Users\krystian\Desktop\";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "image" + ".png", bytes);
            //

        }

        /// <returns>Age of tree on grid node given by x and y</returns>
        public byte GetNoise(int x, int y)
        {
            return noiseGrid[x,y].age;
        }

    }
}