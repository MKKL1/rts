﻿using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{

    public struct MainGrid
    {
        public TerrainResource[,] terrainResourceMap;
        public Vector2 cellSize;
        public Vector2Int size;

        private Vector3 terrainPos;
        public MainGrid(Vector2Int size, Vector2 terrainSize)
        {
            this.size = size;
            terrainResourceMap = new TerrainResource[size.x, size.y];
            cellSize = terrainSize / size;
            terrainPos = new Vector3(0, 0, 0);
        }

        public MainGrid(int sizeX, int sizeY, Vector2 terrainSize) : this(new Vector2Int(sizeX, sizeY), terrainSize) { }

        public MainGrid(int sizeX, int sizeY, float terrainSizeX, float terrainSizeY) : this(sizeX, sizeY, new Vector2(terrainSizeX, terrainSizeY)) {}

        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(terrainPos.x + (x * cellSize.x), terrainPos.y + (y * cellSize.y));
        }
        

    }
}