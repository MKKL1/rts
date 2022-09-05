using Assets.Scripts.TerrainScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    //Based partly on https://github.com/manuelsalvadori/PathfindingUnity/blob/master/Assets/Scripts/AStarSystem.cs

    public struct DiagonalOffset
    {
        public int2 offset;
        public byte condition1;
        public byte condition2;
        public DiagonalOffset(int2 offset, byte c1, byte c2)
        {
            this.offset = offset;
            this.condition1 = c1;
            this.condition2 = c2;
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
            int dx = Math.Abs(x0 - x1);
            int dy = Math.Abs(y0 - y1);
            return 10 * (dx + dy) - 6 * Math.Min(dx, dy);
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
                NativeArray<int2> neighbours = new NativeArray<int2>(8, Allocator.Temp);

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
                        //Go until next parentID is -1 which means it is goal node
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
                    int neighbourCount = GetNeighbours(new int2(currentNode.x, currentNode.y), ref neighbours);
                    for (int i = 0; i < neighbourCount; i++)
                    {
                        int2 neighbourPos = neighbours[i];
                        int arrId = GetInArrayId(neighbourPos.x, neighbourPos.y);
                        if (closedSet[arrId].closed) continue;

                        int moveCostToNeigbour = gScoreCosts[GetInArrayId(currentNode.x, currentNode.y)] + 10;
                        int neighbourGCost = gScoreCosts[arrId];
                        if (neighbourGCost == 0 || moveCostToNeigbour < neighbourGCost)
                        {
                            gScoreCosts[arrId] = moveCostToNeigbour;
                            Node neighbourNode = new Node();
                            neighbourNode.x = neighbourPos.x;
                            neighbourNode.y = neighbourPos.y;
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
                neighbours.Dispose();
            }
            return path;
        }

        private static readonly int2[] straightOffsets = new int2[]
        {
            new int2(-1, 0), //Left
            new int2(1, 0), //Right
            new int2(0, -1), //Bottom
            new int2(0, 1), //Top
        };

        private static readonly DiagonalOffset[] diagonalOffsets = new DiagonalOffset[]
        {
            new DiagonalOffset(new int2(-1, -1), 2, 0), //Bottom Left
            new DiagonalOffset(new int2(1, -1), 2, 1), //Bottom Right
            new DiagonalOffset(new int2(-1, 1), 3, 0), //Top Left
            new DiagonalOffset(new int2(1, 1), 3, 1), //Top Right
        };

        private int GetNeighbours(int2 pos, ref NativeArray<int2> neighbours)
        {
            int neighboursCount = 0;
            //Array of positions that could be moved to
            //Saved by giving proper id of movement like so
            //viableNeighbours[0 for left] <- if true we can move to left
            bool[] viableNeighbours = new bool[straightOffsets.Length];

            //Iterate thru positions left, right, bottom, top in this exact order
            for(int i = 0; i < straightOffsets.Length; i++)
            {
                int2 neighbourPos = pos + straightOffsets[i];
                //Checking if position can be moved to
                if (!isWalkable(neighbourPos)) continue;
                //Saving for later use in diagonal movement
                viableNeighbours[i] = true;
                //Setting to first free space in array
                neighbours[neighboursCount] = neighbourPos;
                neighboursCount++;
            }

            //Bottom left, bottom right, top left, top right
            for (int i = 0; i < diagonalOffsets.Length; i++)
            {
                DiagonalOffset diagonallOffset = diagonalOffsets[i];
                //Checking if we can move to diagonal position
                //for example if we want to move to bottom right we should check bottom and right so 2 and 1 in positions array
                if (viableNeighbours[diagonallOffset.condition1] && viableNeighbours[diagonallOffset.condition2])
                {
                    //Copied from above
                    int2 neighbourPos = pos + diagonallOffset.offset;
                    if (!isWalkable(neighbourPos)) continue;
                    neighbours[neighboursCount] = neighbourPos;
                    neighboursCount++;
                }
            }


            return neighboursCount;
        }

        private bool isWalkable(int2 pos)
        {
            if (pos.x < 0 || pos.x >= gridDataSize.x || pos.y < 0 || pos.y >= gridDataSize.y)
                return false;

            return walkableMap[GetInArrayId(pos.x, pos.y)];
        }
    }
}