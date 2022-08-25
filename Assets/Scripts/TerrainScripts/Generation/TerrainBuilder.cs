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
            terrainGrid.IterateChunks(new System.Action<int, int>((xChunk, yChunk) =>
            {
                terrain.terrainData.SetHeights(xChunk * terrainGrid.chunkSize, yChunk * terrainGrid.chunkSize, terrainGrid.chunks[xChunk, yChunk].heightMap);
            }));
            
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