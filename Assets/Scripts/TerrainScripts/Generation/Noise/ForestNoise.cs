using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation.Noise
{
    class ForestNoiseNode
    {
        public bool isForest = false;
        public byte expensionValue = 0;
        public byte age = 0;
    }

    public class ForestNoise
    {
        private System.Random random;
        public Vector2Int size;
        public int forestAge = 7;
        private ForestNoiseNode[,] noiseGrid;
        private int[,] neighbourNodes = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        public ForestNoise(int sizeX, int sizeY, int seed = 69)
        {
            random = new System.Random(seed);
            size = new Vector2Int(sizeX, sizeY);
            noiseGrid = new ForestNoiseNode[sizeX, sizeY];
            Generate();
        }

        private bool isOccupied(int x, int y)
        {
            return x < 0 || x >= size.x || y < 0 || y >= size.y || noiseGrid[x,y].isForest;
        }



        private void Generate()
        {
            //TODO Remove
            //Debugging
            Texture2D texture = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
            


            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    ForestNoiseNode node = new ForestNoiseNode();
                    if (random.Next(0, 100) < 1)
                    {
                        node.isForest = true;
                        node.expensionValue = (byte)random.Next(5, 10);
                    }
                    else node.isForest = false;
                    noiseGrid[x, y] = node;
                }



            for(int i = forestAge; i >= 0; i--)
                for (int x = 0; x < size.x; x++)
                    for (int y = 0; y < size.y; y++)
                    {
                        ForestNoiseNode node = noiseGrid[x, y];
                        if(node.expensionValue > 0)
                        {
                            
                            node.expensionValue--;
                            for(int a = 0; a < 4; a++)
                            {
                                int eX = x + neighbourNodes[a, 0];
                                int eY = y + neighbourNodes[a, 1];
                                if (isOccupied(eX, eY)) continue;

                                ForestNoiseNode exnode = noiseGrid[eX, eY];
                                exnode.expensionValue = node.expensionValue;
                                exnode.isForest = true;

                                Debug.Log($"{eX} {eY} {exnode.expensionValue}");

                                noiseGrid[eX, eY] = exnode;
                            }
                        }
                        node.age++;
                        noiseGrid[x, y] = node;
                    }




                    //TODO Remove
                    for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {
                    if (noiseGrid[x, y].isForest) texture.SetPixel(x, y, Color.green);
                    else texture.SetPixel(x, y, Color.white);
                }

            texture.Apply();

            Debug.Log("Generated");
            byte[] bytes = texture.EncodeToPNG();
            var dirPath = @"C:\Users\\Desktop\";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "image" + ".png", bytes);
            //

        }

        public byte GetNoise(int x, int y)
        {
            return 0;
        }

    }
}