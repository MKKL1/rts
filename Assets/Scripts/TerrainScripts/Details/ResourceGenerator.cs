using Assets.Scripts.TerrainScripts.Generation.Noise;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public class ResourceGenerator
    {
        private TerrainGenSettings terrainGenSettings;
        private ForestNoise forestNoise;
        private System.Random rnd = new System.Random();
        private MainGrid mainGrid = GameMain.instance.mainGrid;
        private Terrain terrain = GameMain.instance.mainTerrain;
        public ResourceGenerator(TerrainGenSettings data, int seed)
        {
            terrainGenSettings = data;
            forestNoise = new ForestNoise(mainGrid.size.x, mainGrid.size.y, seed)
            {
                forestAge = 10
            };
            forestNoise.Generate();

        }
        /// <param name="x">x on main grid</param>
        /// <param name="y">y on main grid</param>
        /// <returns>Returns null if no resource is on x and y</returns>
        public TerrainResourceNode GetResourceID(int x, int y)
        {
            if(forestNoise.GetNoise(x, y) > 0)
            {
                return new TerrainResourceNode()
                {
                    prefabsList = ResourcePrefabsList.TREE,
                    resourceTypeID = 4
                };
            }
            return null;
        }

        private GameObject GetTree(Vector2 worldPosition, int id)
        {
            GameObject tmp = terrainGenSettings.resourceIDManager.GetDetailByID(ResourcePrefabsList.TREE, id);
            Vector2 v1 = Utils.RandomMove(worldPosition, mainGrid.cellSize.x * 0.5f, mainGrid.cellSize.y * 0.5f);
            Vector3 pos = new Vector3(v1.x, terrain.SampleHeight(new Vector3(v1.x, 0, v1.y)), v1.y);
            tmp.transform.position = pos;
            tmp.transform.rotation = Quaternion.Euler(0, rnd.Next(0, 360), 0);
            return tmp;
        }
    }
}