using Assets.Scripts.TerrainScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Simulation
{
    public class Node : IComparable<Node>
    {
        public int x;
        public int y;
        public Node previousNode;
        public float gScore;
        public float fScore = float.MaxValue;
        public Node(int x, int y, Node prev, float gScore = float.MaxValue)
        {
            this.x = x;
            this.y = y;
            previousNode = prev;
            this.gScore = gScore;
        }

        public int CompareTo(Node other)
        {
            return this.fScore.CompareTo(other.fScore);
        }

        //public static bool operator ==(Node node, Vector2Int vector)
        //{
        //    return node.x == vector.x && node.y == vector.y;
        //}

        //public static bool operator !=(Node node, Vector2Int vector)
        //{
        //    return node.x != vector.x || node.y != vector.y;
        //}

        public Vector2Int asVector()
        {
            return new Vector2Int(x, y);
        }

    }

    public class AStarAlgorithm
    {
        private MainGrid mainGrid;
        private Vector2Int gridDataSize;
        private List<Vector2Int> closedSet;

        private float h(Vector2Int current, Vector2Int goal)
        {
            return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
        }

        public AStarAlgorithm(MainGrid mainGrid)
        {
            this.mainGrid = mainGrid;
            gridDataSize = mainGrid.gridDataSize;
        }

        //Algorithm from wikipedia
        public Stack<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            Debug.Log($"[A*] start {start}, goal {goal}");
            C5.IntervalHeap<Node> openSet = new C5.IntervalHeap<Node>();
            closedSet = new List<Vector2Int>();
            Node startNode = new Node(start.x, start.y, null, 0f);
            startNode.fScore = 0f;
            openSet.Add(startNode);

            while(!openSet.IsEmpty)
            {
                Node currentNode = openSet.FindMin();
                Debug.Log($"[A*] Node {currentNode.asVector()} current");
                if(currentNode.asVector() == goal)
                {
                    Stack<Vector2Int> path = new Stack<Vector2Int>();
                    path.Push(currentNode.asVector());
                    Node previousNode = currentNode.previousNode;
                    while(true)
                    {
                        path.Push(previousNode.asVector());
                        previousNode = currentNode.previousNode;
                        if (previousNode == null) break;
                    }
                    return path;
                }
                //Delete current node, which is min. In short
                openSet.DeleteMin();

                closedSet.Add(new Vector2Int(currentNode.x, currentNode.y));
                foreach(Node neighbourNode in getNeighbours(currentNode))
                {
                    float tentative_gScore = currentNode.gScore + 1f;
                    if(tentative_gScore < neighbourNode.gScore)
                    {
                        neighbourNode.previousNode = currentNode;
                        neighbourNode.gScore = tentative_gScore;
                        neighbourNode.fScore = tentative_gScore + h(currentNode.asVector(), goal);
                        openSet.Add(neighbourNode);
                    }
                }
            }

            return null;
        }

        private static readonly int[,] neighbours = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        private List<Node> getNeighbours(Node centerNode)
        {
            List<Node> neighbourNodes = new List<Node>();
            for(int i = 0; i < neighbours.GetLength(0); i++)
            {
                int xn = centerNode.x - neighbours[i, 0];
                int yn = centerNode.y - neighbours[i, 1];
                if (xn > 0 && xn < gridDataSize.x && yn > 0 && yn < gridDataSize.y)
                {
                    Vector2Int inChunk = mainGrid.GetInChunkOffset(xn, yn);
                    //TODO may cause performance issues
                    if (mainGrid.GetChunkAt(xn, yn).walkableMap[inChunk.x, inChunk.y])
                    {
                        if (!closedSet.Contains(new Vector2Int(xn, yn)))
                            neighbourNodes.Add(new Node(xn, yn, null));
                    }
                }
            }
            return neighbourNodes;
        }
    }
}