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
            float forestHeight = forestNoise.GetNoise(x, y);
            if (forestHeight > 0)
            {
                byte treeid = 0;
                if (forestHeight > 0 && forestHeight <= 3) treeid = 3;
                else if (forestHeight >= 4 && forestHeight <= 6) treeid = 2;
                else if (forestHeight >= 7 && forestHeight <= 9) treeid = 1;
                else if (forestHeight >= 10 && forestHeight <= 13) treeid = 0;
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