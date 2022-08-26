using Assets.Scripts.TerrainScripts.Details;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public struct InitialDataMainGrid
    {
        public Vector2Int gridDataSize;
        public Vector2 cellSize;
    }

    public class MainGrid : GridBase<MainGridChunk>
    {
        public MainGrid(int gridDataSizeX, int gridDataSizeY, Vector2 terrainSize)
            : base(
                  gridDataSizeX, 
                  gridDataSizeY, 
                  terrainSize.x / gridDataSizeX,
                  terrainSize.y / gridDataSizeY) { }
        public MainGrid(int gridDataSizeX, int gridDataSizeY, float cellSize)
            : base(gridDataSizeX,gridDataSizeY,cellSize,cellSize) { }

        public InitialDataMainGrid GetInitialData()
        {
            return new InitialDataMainGrid()
            {
                gridDataSize = gridDataSize,
                cellSize = worldCellSize
            };
        }
    }

    public static class MainGridNetWriteRead
    {
        public static void WriteInitialMainGrid(this NetworkWriter networkWriter, InitialDataMainGrid value)
        {
            networkWriter.WriteVector2Int(value.gridDataSize);
            networkWriter.WriteVector2(value.cellSize);
        }

        public static InitialDataMainGrid ReadInitialMainGrid(this NetworkReader networkReader)
        {
            return new InitialDataMainGrid()
            {
                gridDataSize = networkReader.ReadVector2Int(),
                cellSize = networkReader.ReadVector2()
            };
        }
    }
}