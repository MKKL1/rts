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
        private MainGrid mainGrid = GameMain.instance.mainGrid;
        private Terrain terrain = GameMain.instance.mainTerrain;
        //TODO remove?
        private BiomesManager biomesManager;
        private TerrainGrid terrainGrid;
        private int rockCount;
        private int goldOreCount;
        public ResourceGenerator(TerrainGenSettings data, BiomesManager biomesManager,TerrainGrid terrainGrid, int seed)
        {
            terrainGenSettings = data;
            rockCount = terrainGenSettings.resourceIDManager.rocks.Count;
            goldOreCount = terrainGenSettings.resourceIDManager.goldOre.Count;

            this.biomesManager = biomesManager;
            this.terrainGrid = terrainGrid;
            forestNoise = new ForestNoise(mainGrid.size.x, mainGrid.size.y, seed)
            {
                forestAge = 10
            };
            forestNoise.Generate();

            rockVeins = new VeinNoise(mainGrid.size.x, mainGrid.size.y, seed);
            rockVeins.canPlaceVein = canPlaceVein;
            rockVeins.Generate(50);

            goldVeins = new VeinNoise(mainGrid.size.x, mainGrid.size.y, seed+1);
            goldVeins.canPlaceVein = canPlaceVein;
            goldVeins.Generate(50);
        }

        private bool canPlaceVein(Vector2Int pos)
        {
            return forestNoise.GetNoise(pos.x, pos.y) == 0 || 
                !biomesManager.GetBiome(terrainGrid.GetCellAtWorldPos(mainGrid.GetWorldPosition(pos.x, pos.y)).biome).biomeData.resources;
        }
        /// <param name="x">x on main grid</param>
        /// <param name="y">y on main grid</param>
        /// <returns>Returns null if no resource is on x and y</returns>
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

            
            return null;
        }
    }
}