using Assets.Scripts.TerrainScripts.Details;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts
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

        public void SetHeightMap(byte[,] heightMap)
        {
            float[,] floatHeightMap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];
            for (int i = 0; i < heightMap.GetLength(0); i++)
                for (int j = 0; j < heightMap.GetLength(1); j++)
                {
                    floatHeightMap[i,j] = heightMap[i,j] / 255f;
                }
            SetHeightMap(floatHeightMap);
        }

        public void SetHeightMap(float[,] heightMap)
        {
            terrain.terrainData.SetHeights(0, 0, heightMap);
        }

        public void SetResources(Transform featuresTransform, TerrainResourceNode[,] terrainResourceMap)
        {
            for (int i = 0; i < terrainResourceMap.GetLength(0); i++)
                for (int j = 0; j < terrainResourceMap.GetLength(1); j++)
                {
                    TerrainResourceNode prefabsList = terrainResourceMap[i, j];
                    GameObject tmp = settings.resourceIDManager.GetDetailByID(ResourcePrefabsList.TREE, prefabsList.resourceTypeID);
                    Vector2 v1 = Utils.RandomMove(mainGrid.GetWorldPosition(i, j), mainGrid.cellSize.x * 0.5f, mainGrid.cellSize.y * 0.5f);
                    Vector3 pos = new Vector3(v1.x, terrain.SampleHeight(new Vector3(v1.x, 0, v1.y)), v1.y);

                    GameObject ins = Object.Instantiate(tmp, pos, Quaternion.Euler(0, Random.Range(0, 359), 0));
                    ins.transform.parent = featuresTransform;
                }
        }
    }
}