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

    public struct Node : IComparer<Node>, IEquatable<Node>, IEquatable<Vector2Int>
    {
        public int x;
        public int y;
        public int fScore;
        public int gScore;
        public bool closed;
        public int cameFromNodeID;

        public int Compare(Node x, Node y)
        {
            return x.fScore.CompareTo(y.fScore);
        }

        public bool Equals(Node other)
        {
            return x == other.x && y == other.y;
        }

        public bool Equals(Vector2Int vector)
        {
            return x == vector.x && y == vector.y;
        }
    }

    public class AStarAlgorithm
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
            NativeArray<OffsetWithCost> neighbourOffsets = new NativeArray<OffsetWithCost>(new OffsetWithCost[]
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


            Stack<Vector2Int> path = null;
            NativeArray<Node> nodeArray = new NativeArray<Node>(gridDataSize.x * gridDataSize.y, Allocator.Temp);
            for(int x = 0; x < gridDataSize.x; x++)
                for(int y = 0; y < gridDataSize.y; y++)
                {
                    nodeArray[GetInArrayId(x, y)] = new Node()
                    {
                        x = x,
                        y = y,
                        fScore = int.MaxValue,
                        gScore = int.MaxValue,
                        closed = false,
                        cameFromNodeID = -1
                    };
                }


            NativeHeap<Node, Node> openList = new NativeHeap<Node, Node>(Allocator.Temp);

            openList.Capacity = 65565;
            openList.Insert(new Node()
            {
                x = start.x,
                y = start.y,
                fScore = h(start.x, start.y, goal.x, goal.y),
                gScore = 0,
                cameFromNodeID = -1
            });
            int a = 9999;
            int b = 999;
            while (openList.Count > 0 && a >= 0)
            {
                Node currentNode = openList.Pop();
                Debug.Log($"Node [{currentNode.x}, {currentNode.y}]");
                if (currentNode.Equals(goal))
                {
                    //Reconstruct path
                    path = new Stack<Vector2Int>();
                    Node pathNode = currentNode;
                    while(b >= 0)
                    {
                        path.Push(new Vector2Int(pathNode.x, pathNode.y));
                        int nextId = pathNode.cameFromNodeID;
                        if (nextId == -1) break;
                        pathNode = nodeArray[nextId];
                        b--;
                    }
                    break;
                }

                for (int i = 0; i < neighbourOffsets.Length; i++)
                {
                    
                    int2 neigbhourPos = new int2(currentNode.x + neighbourOffsets[i].offset.x, currentNode.y + neighbourOffsets[i].offset.y);
                    if (neigbhourPos.x < 0 || neigbhourPos.x >= gridDataSize.x || neigbhourPos.y < 0 || neigbhourPos.y >= gridDataSize.y)
                        continue;

                    int arrId = GetInArrayId(neigbhourPos.x, neigbhourPos.y);
                    Node neighbourNode = nodeArray[arrId];
                    Debug.Log($"Node {neigbhourPos.x} {neigbhourPos.y}");

                    if (!walkableMap[arrId] || neighbourNode.closed)
                        continue;

                    int moveCostToNeigbour = currentNode.gScore + neighbourOffsets[i].cost;
                    if (moveCostToNeigbour < neighbourNode.gScore || !openList.Contains(neighbourNode))
                    {
                        neighbourNode.gScore = moveCostToNeigbour;
                        neighbourNode.fScore = moveCostToNeigbour + h(neighbourNode.x, neighbourNode.y, goal.x, goal.y);
                        neighbourNode.cameFromNodeID = arrId;
                        openList.Insert(neighbourNode);
                    }
                }
                a--;
            }

            openList.Dispose();
            nodeArray.Dispose();
            neighbourOffsets.Dispose();
            return path;
        }
    }
}