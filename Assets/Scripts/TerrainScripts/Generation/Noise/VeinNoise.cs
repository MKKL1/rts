using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation.Noise
{
    public struct VeinNoiseNode
    {
        public byte value;
        public bool surrounded;
    }

    public class VeinNoise
    {
        //private List<Vector2> veinCenterList = new List<Vector2>();
        private VeinNoiseNode[,] noiseGrid;
        private System.Random rnd;
        private int[,] neighbourNodes = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        public Func<Vector2Int, bool> canPlaceVein = null;
        public Vector2Int size;
        public int veinSizeMin = 4;
        public int veinSizeMax = 10;

        public VeinNoise(int sizeX, int sizeY, int seed = 50)
        {
            size = new Vector2Int(sizeX, sizeY);
            noiseGrid = new VeinNoiseNode[sizeX, sizeY];
            rnd = new System.Random(seed);
        }

        public byte GetNoise(int x, int y)
        {
            return noiseGrid[x,y].value;
        }

        private void SetNewNode(Vector2Int pos)
        {
            SetNewNode(pos.x, pos.y);
        }

        private void SetNewNode(int x, int y)
        {
            noiseGrid[x, y] = new VeinNoiseNode()
            {
                value = 1,
                surrounded = false
            };
        }

        private bool insideTerrainBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < size.x && y < size.y;
        }

        //TODO doesn't seem to work
        //TODO needs to be faster and guarantee that solution is found
        private Vector2Int GetRandomNode()
        {
            int i = 0;
            Vector2Int rndPos = default;
            while(i < 5)
            {
                rndPos = new Vector2Int(rnd.Next(0, size.x), rnd.Next(0, size.y));
                if (canPlaceVein == null || canPlaceVein.Invoke(rndPos)) break;
                i++;
            }
            return rndPos;
        }

        public void Generate(int count)
        {
            for(int i = 0; i < count; i++)
            {
                int currentSize = rnd.Next(veinSizeMin, veinSizeMax + 1);
                Vector2Int pos = GetRandomNode();
                List<Vector2Int> currentVein = new List<Vector2Int>();
                currentVein.Add(pos);
                SetNewNode(pos);

                while(currentSize > 0)
                {
                    //Choose random node from current vein that isn't surrounded
                    Vector2Int choosenNode;
                    while (true)
                    {
                        choosenNode = currentVein[rnd.Next(0, currentVein.Count)];
                        if (!noiseGrid[choosenNode.x, choosenNode.y].surrounded) break;
                    }
                    //Chose random neighbour of choosen node
                    int[] randomNodeOrder = Utils.randomOrder(4, rnd);
                    for(int j = 0; j < 4; j++)
                    {
                        int x = choosenNode.x + neighbourNodes[randomNodeOrder[j], 0];
                        int y = choosenNode.y + neighbourNodes[randomNodeOrder[j], 1];
                        if (!insideTerrainBounds(x, y)) break;
                        if (noiseGrid[x, y].value == 0)
                        {
                            currentVein.Add(new Vector2Int(x, y));
                            SetNewNode(x,y);
                            break;
                        }
                        if (j == 3)
                        {
                            noiseGrid[x, y].surrounded = true;
                        }
                    }
                    currentSize--;
                }

            }
        }
    }
}