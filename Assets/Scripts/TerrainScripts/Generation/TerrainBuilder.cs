using Assets.Scripts.DebugTools;
using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Generation
{
    public class TerrainBuilder
    {
        private Terrain terrain;
        private TerrainGenSettings settings;
        private MainGrid mainGrid;
        public TerrainBuilder(TerrainGenSettings settings, Terrain terrain, MainGrid mainGrid)
        {
            this.terrain = terrain;
            this.settings = settings;
            this.mainGrid = mainGrid;
        }

        public void SetHeightMap(TerrainGrid terrainGrid)
        {
            float[,] reversedHeightMap = new float[terrainGrid.gridDataSize.x, terrainGrid.gridDataSize.y];
            DebugTexture debugTexture = new DebugTexture(terrainGrid.gridDataSize.x, terrainGrid.gridDataSize.y);
            terrainGrid.IterateChunks(new System.Action<int, int>((xChunk, yChunk) =>
            {
                TerrainChunk currentChunk = terrainGrid.chunks[xChunk, yChunk];
                terrainGrid.IterateInChunk(currentChunk, new System.Action<int, int>((xInChunk, yInChunk) =>
                {
                    int xOnGrid = (xChunk * terrainGrid.chunkSize) + xInChunk;
                    int yOnGrid = (yChunk * terrainGrid.chunkSize) + yInChunk;

                    reversedHeightMap[yOnGrid, xOnGrid] = currentChunk.heightMap[xInChunk, yInChunk];
                    debugTexture.SetPixel(xOnGrid, yOnGrid, currentChunk.heightMap[xInChunk, yInChunk]);
                }));
            }));

            debugTexture.SaveToPath("DebugTexture/", "height");

            terrain.terrainData.SetHeights(0,0,reversedHeightMap);
        }

        public void SetResources(Transform featuresTransform)
        {
            mainGrid.IterateChunks(new System.Action<int, int>((xChunk, yChunk) =>
            {
                MainGridChunk currentChunk = mainGrid.chunks[xChunk, yChunk];
                mainGrid.IterateInChunk(xChunk, yChunk, new System.Action<int, int>((xInChunk, yInChunk) =>
                {
                    TerrainResourceNode resourceNode = currentChunk.resourceMap[xInChunk, yInChunk];
                    if (!resourceNode.isEmpty())
                    {
                        int gridPosX = xInChunk + (xChunk * mainGrid.chunkSize);
                        int gridPosY = yInChunk + (yChunk * mainGrid.chunkSize);

                        GameObject tmp = settings.resourceIDManager.GetDetailByID(resourceNode.prefabsList, resourceNode.resourceTypeID);
                        Vector2 v1 = Utils.RandomMove(mainGrid.GetWorldPosition(gridPosX, gridPosY), mainGrid.worldCellSize.x * 0.4f, mainGrid.worldCellSize.y * 0.4f);
                        Vector3 pos = new Vector3(v1.x, terrain.SampleHeight(new Vector3(v1.x, 0, v1.y)), v1.y);

                        GameObject ins = Object.Instantiate(tmp, pos, Quaternion.Euler(0, Random.Range(0, 359), 0));
                        ins.transform.parent = featuresTransform;
                        TerrainResource res = ins.GetComponent<TerrainResource>();
                        if (res != null)
                        {
                            res.gridPosX = (short)gridPosX;
                            res.gridPosY = (short)gridPosY;
                            currentChunk.spawnedResourceMap[xInChunk, yInChunk] = res;
                        }
                    }
                }));
                
            }));
                
        }

        public void BuildTerrain(TerrainGrid terrainGrid, Transform featuresTransform)
        {
            SetHeightMap(terrainGrid);
            SetResources(featuresTransform);
        }
    }
}