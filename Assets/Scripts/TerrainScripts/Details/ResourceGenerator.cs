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
        private VeinNoise rockVeins;
        private VeinNoise goldVeins;
        private System.Random rnd = new System.Random();
        private TerrainGrid terrainGrid;
        private int rockCount;
        private int goldOreCount;

        /// <summary>
        /// Handles generation of resource map from which resources can be placed on terrain
        /// </summary>
        public ResourceGenerator(Vector2Int size, TerrainGenSettings data, int seed)
        {
            terrainGenSettings = data;
            rockCount = terrainGenSettings.resourceIDManager.rocks.Count;
            goldOreCount = terrainGenSettings.resourceIDManager.goldOre.Count;

            forestNoise = new ForestNoise(size.x, size.y, seed)
            {
                forestAge = 10
            };
            forestNoise.Generate();

            rockVeins = new VeinNoise(size.x, size.y, seed);
            rockVeins.Generate(50);

            goldVeins = new VeinNoise(size.x, size.y, seed+1);
            goldVeins.Generate(50);
        }
        /// <param name="x">x on main grid</param>
        /// <param name="y">y on main grid</param>
        public TerrainResourceNode GetResourceID(int x, int y)
        {
            int treeAge = forestNoise.GetNoise(x, y);
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

            int goldSize = goldVeins.GetNoise(x, y);
            if (goldSize > 0)
            {
                return new TerrainResourceNode()
                {
                    prefabsList = ResourcePrefabsList.GOLD,
                    resourceTypeID = (byte)rnd.Next(0, goldOreCount)
                };
            }

            int rockSize = rockVeins.GetNoise(x, y);
            if(rockSize > 0)
            {
                return new TerrainResourceNode()
                {
                    prefabsList = ResourcePrefabsList.ROCK,
                    resourceTypeID = (byte)rnd.Next(0, rockCount)
                };
            }


            return new TerrainResourceNode()
            {
                prefabsList = ResourcePrefabsList.NONE,
                resourceTypeID = 0
            };
        }
    }
}