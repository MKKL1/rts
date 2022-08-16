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
        private BiomesManager biomesManager;
        public ResourceGenerator(TerrainGenSettings data, BiomesManager biomesManager, int seed)
        {
            terrainGenSettings = data;
            this.biomesManager = biomesManager;
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
            float treeAge = forestNoise.GetNoise(x, y);
            if (treeAge > 0)
            {
                byte treeid = 0;
                if (treeAge > 0 && treeAge <= 3) treeid = 0;
                else if (treeAge >= 4 && treeAge <= 6) treeid = 1;
                else if (treeAge >= 7 && treeAge <= 9) treeid = 2;
                else if (treeAge >= 10 && treeAge <= 11) treeid = 3;
                else if (treeAge >= 12) treeid = 4;

                return new TerrainResourceNode()
                {
                    prefabsList = ResourcePrefabsList.TREE,
                    resourceTypeID = treeid
                };
            }
            return null;
        }
    }
}