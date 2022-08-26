using Mirror;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public struct InitialDataTerrainGrid
    {
        public Vector2Int gridDataSize;
        public Vector2 cellSize;
    }
    public class TerrainGrid : GridBase<TerrainChunk>
    {
        public TerrainGrid(int gridDataSizeX, int gridDataSizeY, float cellSizeX, float cellSizeY)
            :base(gridDataSizeX, gridDataSizeY, cellSizeX, cellSizeY) {}

        public BiomeType GetBiomeAt(int x, int y)
        {
            Vector2Int offset = GetInChunkOffset(x, y);
            return GetChunkAt(x, y).biomeGrid[offset.x, offset.y];
        }

        public BiomeType GetBiomeAt(Vector2Int vector) { return GetBiomeAt(vector.x, vector.y); }

        public BiomeType GetBiomeAtWorldPos(float x, float y)
        {
            
            Vector2Int offset = GetWorldPosInChunkOffset(x, y);
            return GetChunkAtWorldPos(x, y).biomeGrid[offset.x, offset.y];
        }
        public BiomeType GetBiomeAtWorldPos(Vector2 pos) { return GetBiomeAtWorldPos(pos.x, pos.y); }
        public BiomeType GetBiomeAtWorldPos(Vector3 pos) { return GetBiomeAtWorldPos(pos.x, pos.z); }

        public InitialDataTerrainGrid GetInitialData()
        {
            return new InitialDataTerrainGrid()
            {
                gridDataSize = gridDataSize,
                cellSize = worldCellSize
            };
        }
    }

    public static class TerrainGridNetWriteRead
    {
        public static void WriteInitialTerrGrid(this NetworkWriter networkWriter, InitialDataTerrainGrid value)
        {
            networkWriter.WriteVector2Int(value.gridDataSize);
            networkWriter.WriteVector2(value.cellSize);
        }

        public static InitialDataTerrainGrid ReadInitialTerrGrid(this NetworkReader networkReader)
        {
            return new InitialDataTerrainGrid()
            {
                gridDataSize = networkReader.ReadVector2Int(),
                cellSize = networkReader.ReadVector2()
            };
        }
    }
}