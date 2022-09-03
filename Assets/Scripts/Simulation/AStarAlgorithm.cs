using Assets.Scripts.TerrainScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Simulation
{

    public struct OffsetWithCost
    {
        public int2 offset;
        public ushort cost;
        public OffsetWithCost(int2 offset, ushort cost)
        {
            this.offset = offset;
            this.cost = cost;
        }
    }

    public unsafe struct Node : IComparer<Node>
    {
        public int x;
        public int y;
        public int fScore;
        public Node* cameFromNodeId;

        public int Compare(Node x, Node y)
        {
            return x.fScore.CompareTo(y.fScore);
        }
        public bool isEqual(Vector2Int vector)
        {
            return x == vector.x && y == vector.y;
        }
    }

    public unsafe class AStarAlgorithm
    {
        private NativeArray<bool> walkableMap;
        private int2 gridDataSize;

        private int h(int x0, int y0, int x1, int y1)
        {
            return Math.Abs(x0-x1) + Math.Abs(y0 - y1) * 10;
        }

        private int GetInArrayId(int x, int y)
        {
            return y * gridDataSize.x + x;
        }

        public AStarAlgorithm(NativeArray<bool> walkableMap, int2 gridSize)
        {
            this.walkableMap = walkableMap;
            gridDataSize = gridSize;
        }


        //Algorithm from wikipedia
        public Stack<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            NativeArray<int> gScoreArray = new NativeArray<int>(gridDataSize.x * gridDataSize.y, Allocator.Temp);
            NativeHeap<Node, Node> openList = new NativeHeap<Node, Node>(Allocator.Temp);
            for(int i = 0; i < gridDataSize.x; i++)
                for(int j = 0; j < gridDataSize.y; j++)
                {
                    gScoreArray[GetInArrayId()] = int.MaxValue;
                }
            NativeList<Node> closedList = new NativeList<Node>(Allocator.Temp);
            openList.Capacity = 65565;
            openList.Insert(new Node()
            {
                x = start.x,
                y = start.y,
                fScore = h(start.x, start.y, goal.x, goal.y),
                cameFromNodeId = null
            });
            gScoreArray[start.x, start.y] = 0;
            while (openList.Count > 0)
            {
                Node currentNode = openList.Pop();
                if (currentNode.isEqual(goal))
                {
                    //Reconstruct path
                }

                for(int i = 0; i < neighbourOffsets.Length; i++)
                {
                    int2 neighourPos = new int2(currentNode.x + neighbourOffsets[i].offset.x, currentNode.y + neighbourOffsets[i].offset.y);
                    if(neighourPos.x > 0 && neighourPos.x < gridDataSize.x && neighourPos.y > 0 && neighourPos.y < gridDataSize.y)
                    {
                        if (walkableMap[GetInArrayId(neighourPos.x, neighourPos.y)])
                        {
                            int tentative_gScore = gScoreArray[neighourPos.x, neighourPos.y] + neighbourOffsets[i].cost;
                            //TODO searching in heap
                            if(tentative_gScore < gScoreArray[neighourPos.x, neighourPos.y])
                            {
                                Node neighbourNode = new Node();
                                neighbourNode.x = neighourPos.x;
                                neighbourNode.y = neighourPos.y;
                                gScoreArray[neighbourNode.x, neighbourNode.y] = tentative_gScore;
                                neighbourNode.fScore = tentative_gScore + h(neighbourNode.x, neighbourNode.y, goal.x, goal.y);
                                if()
                            }
                        }
                    }
                }
            }

            openList.Dispose();
            closedList.Dispose();
            return null;
        }

        private NativeArray<OffsetWithCost> neighbourOffsets = new NativeArray<OffsetWithCost>(new OffsetWithCost[]
        {
            new OffsetWithCost(new int2(-1, 0), 10), //Left
            new OffsetWithCost(new int2(1, 0), 10), //Right
            new OffsetWithCost(new int2(0, -1), 10), //Bottom
            new OffsetWithCost(new int2(0, 1), 10), //Top
            new OffsetWithCost(new int2(-1, -1), 14), //Bottom Left
            new OffsetWithCost(new int2(1, -1), 14), //Bottom Right
            new OffsetWithCost(new int2(-1, 1), 14), //Top Left
            new OffsetWithCost(new int2(1, 1), 14), //Top Right
        }, Allocator.Temp);
    }
}