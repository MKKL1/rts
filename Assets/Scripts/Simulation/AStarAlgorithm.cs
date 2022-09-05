using Assets.Scripts.TerrainScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    //Based partly on https://github.com/manuelsalvadori/PathfindingUnity/blob/master/Assets/Scripts/AStarSystem.cs
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

    public unsafe struct Node : IComparer<Node>, IEquatable<Node>, IEquatable<Vector2Int>
    {
        public int x;
        public int y;
        public int fScore;
        public bool closed;
        public int parentID;

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

        public override string ToString()
        {
            return $"Node({x},{y})";
        }

        public string ToStringAll()
        {
            return $"{this.ToString()}, fScore = {fScore}, closed = {closed}, parentId = {parentID}";
        }
    }

    public class AStarAlgorithm
    {
        //TODO handle with timeout
        private int iterationLimit = 3000;
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

        public Stack<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            Stack<Vector2Int> path = null;
            if (!walkableMap[GetInArrayId(goal.x, goal.y)])
            {
                Debug.Log("Goal was blocked");
                return null;
            }


            //TODO catch any unsafe memory access
            unsafe
            {
                int _iterationLimit = iterationLimit;
                //TODO move outside of this method and make static
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

                NativeArray<int> gScoreCosts = new NativeArray<int>(gridDataSize.x * gridDataSize.y, Allocator.Temp);
                NativeHeap<Node, Node> openList = new NativeHeap<Node, Node>(Allocator.Temp);
                NativeArray<Node> closedSet = new NativeArray<Node>(gridDataSize.x * gridDataSize.y, Allocator.Temp);
                openList.Capacity = 65565;
                openList.Insert(new Node()
                {
                    x = start.x,
                    y = start.y,
                    fScore = h(start.x, start.y, goal.x, goal.y),
                    parentID = -1
                });

                while (openList.Count > 0 && _iterationLimit >= 0)
                {
                    
                    Node currentNode = openList.Pop();
                    currentNode.closed = true;
                    int currendID = GetInArrayId(currentNode.x, currentNode.y);
                    closedSet[currendID] = currentNode;
                    //Check if algorithm found goal node
                    if (currentNode.Equals(goal))
                    {
                        //Reconstruct path
                        path = new Stack<Vector2Int>();
                        int pathNodeID = currendID;

                        //TODO estimate maximal path length
                        while (true)
                        {
                            Node pn = closedSet[pathNodeID];
                            path.Push(new Vector2Int(pn.x, pn.y));
                            pathNodeID = pn.parentID;

                            if (pathNodeID == -1) break;
                        }
                        break;
                    }

                    //Iterate thru neighbours of current node
                    for (int i = 0; i < neighbourOffsets.Length; i++)
                    {
                        
                        //Position of neighbour node
                        int2 neigbhourPos = new int2(currentNode.x + neighbourOffsets[i].offset.x, currentNode.y + neighbourOffsets[i].offset.y);
                        if (neigbhourPos.x < 0 || neigbhourPos.x >= gridDataSize.x || neigbhourPos.y < 0 || neigbhourPos.y >= gridDataSize.y)
                            continue;

                        int arrId = GetInArrayId(neigbhourPos.x, neigbhourPos.y);

                        if (closedSet[arrId].closed || !walkableMap[arrId])
                            continue;

                        int moveCostToNeigbour = gScoreCosts[GetInArrayId(currentNode.x, currentNode.y)] + neighbourOffsets[i].cost;
                        int neighbourGCost = gScoreCosts[arrId];
                        if (neighbourGCost == 0 || moveCostToNeigbour < neighbourGCost)
                        {
                            gScoreCosts[arrId] = moveCostToNeigbour;
                            Node neighbourNode = new Node();
                            neighbourNode.x = neigbhourPos.x;
                            neighbourNode.y = neigbhourPos.y;
                            neighbourNode.fScore = moveCostToNeigbour + h(neighbourNode.x, neighbourNode.y, goal.x, goal.y);
                            neighbourNode.parentID = currendID;

                            //TODO update node instead of removing it
                            if (openList.Contains(neighbourNode)) continue;

                            openList.Insert(neighbourNode);
                        }
                    }
                    _iterationLimit--;
                }

                openList.Dispose();
                closedSet.Dispose();
                gScoreCosts.Dispose();
                neighbourOffsets.Dispose();
            }
            return path;
        }
    }
}