using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation.Noise
{
    //TODO
    //Generate forest by ground quality value to better simulate natural forest
    class ForestNoiseNode : ICloneable
    {
        public bool isForest = false;
        public byte expensionValue = 0;
        public byte age = 0;
        //public byte groundQuality = 0;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ForestLevels : IComparable<ForestLevels>
    {
        public float minHeight;
        public int forestPercentage;
        public int minExpansion;
        public int maxExpansion;

        public int CompareTo(ForestLevels obj)
        {
            return minHeight.CompareTo(obj.minHeight);
        }

        public override string ToString()
        {
            return $"{minHeight}: {forestPercentage}% ({minExpansion}, {maxExpansion})";
        }
    }

    public class ForestNoise
    {
        public byte forestAge = 7;
        public float forestNoiseFrequency = 0.015f;
        public List<ForestLevels> forestLevels = new List<ForestLevels>();

        private System.Random random;
        public Vector2Int size;
        private ForestNoiseNode[,] noiseGrid;
        private int[,] neighbourNodes = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        private FastNoiseLite noise;

        public ForestNoise(int sizeX, int sizeY, int seed = 69)
        {
            noise = new FastNoiseLite(seed + 1);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFrequency(forestNoiseFrequency);

            random = new System.Random(seed);
            size = new Vector2Int(sizeX, sizeY);
            noiseGrid = new ForestNoiseNode[sizeX, sizeY];
            forestLevels.Add(new ForestLevels()
            {
                minHeight = 0.5f,
                forestPercentage = 7,
                minExpansion = 3,
                maxExpansion = 12
            });

            forestLevels.Add(new ForestLevels()
            {
                minHeight = 0f,
                forestPercentage = 4,
                minExpansion = 0,
                maxExpansion = 6
            });
        }

        public void SetSeed(int seed) => noise.SetSeed(seed);

        /// <returns>Age of tree on grid node given by x and y</returns>
        public byte GetNoise(int x, int y) => noiseGrid[x, y].age;

        public void Generate()
        {
            //Descending sorting
            forestLevels.Sort((a, b) => b.CompareTo(a));

            //Initial forest grid positioning
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                {

                    ForestNoiseNode node = new ForestNoiseNode();
                    float height = noise.GetNoise(x, y);

                    foreach(ForestLevels forestLevel in forestLevels)
                    {
                        if(height > forestLevel.minHeight)
                        {
                            if(random.Next(0, 100) < forestLevel.forestPercentage)
                            {
                                node.isForest = true;
                                node.expensionValue = (byte)random.Next(forestLevel.minExpansion, forestLevel.maxExpansion);
                            }
                            break;
                        }
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
                            
                            int occupiedNeighbours = 0;
                            int checkedNeighbours = 0;
                            //Choose random number of nodes to check (possibly expand to)
                            int randomNeighboursCheck = random.Next(1, 3);
                            //Generate random order of checking neighbours
                            int[] randomNodeOrder = Utils.randomOrder(4, random);
                            for (int a = 0; a < 4 && checkedNeighbours <= randomNeighboursCheck; a++)
                            {
                                checkedNeighbours++;
                                //Expanding in one random direction
                                int eX = x + neighbourNodes[randomNodeOrder[a], 0];
                                int eY = y + neighbourNodes[randomNodeOrder[a], 1];
                                //Checking if there is already forest on node
                                if (isOccupied(eX, eY))
                                {
                                    occupiedNeighbours++;
                                    continue;
                                }

                                //If node is empty create new child node
                                ForestNoiseNode exnode = (ForestNoiseNode)noiseGrid[eX, eY].Clone();
                                exnode.expensionValue = (byte)(node.expensionValue-1);
                                exnode.isForest = true;
                                newNoiseGrid[eX, eY] = exnode;
                            }
                            if (occupiedNeighbours == 4) 
                                //If node is surrounded it cannot expand
                                newNoiseGrid[x,y].expensionValue = 0;
                            //If it is not surrounded remove 1 expansion point
                            else newNoiseGrid[x, y].expensionValue--;
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

        }

        private bool isOccupied(int x, int y)
        {
            return x < 0 || x >= size.x || y < 0 || y >= size.y || noiseGrid[x, y].isForest;
        }

    }
}