using UnityEngine;

namespace Assets.Scripts.TerrainScripts
{
    public class TerrainGrid : GridBase<TerrainChunk>
    {
        public TerrainGrid(int chunkDataSizeX, int chunkDataSizeY, float cellSizeX, float cellSizeY)
            :base(chunkDataSizeX, chunkDataSizeY, cellSizeX, cellSizeY) {}

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
    }
}