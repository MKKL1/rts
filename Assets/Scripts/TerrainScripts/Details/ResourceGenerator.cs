using Assets.Scripts.TerrainScripts.Generation.Noise;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TerrainScripts.Details
{
    public class ResourceGenerator
    {
        private TerrainGenSettings terrainGenSettings;
        private FastNoiseLite treeNoise;
        private System.Random rnd = new System.Random();
        private MainGrid mainGrid = GameMain.instance.mainGrid;
        private Terrain terrain = GameMain.instance.mainTerrain;
        public ResourceGenerator(TerrainGenSettings data, int seed)
        {
            terrainGenSettings = data;
            treeNoise = new FastNoiseLite(seed+5);
            treeNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2); 
            treeNoise.SetFrequency(0.01f);

            
        }

        public GameObject GetResource(int x, int y, Vector2 worldPosition)
        {
            if(treeNoise.GetNoise(x, y) > 0.8f)
            {
                return GetTree(x, y, worldPosition);
            }
            return null;
        }

        private GameObject GetTree(int x, int y, Vector2 worldPosition)
        {
            GameObject tmp = terrainGenSettings.trees[4];
            Vector2 v1 = Utils.RandomMove(worldPosition, mainGrid.cellSize.x * 0.5f, mainGrid.cellSize.y * 0.5f);
            Vector3 pos = new Vector3(v1.x, terrain.SampleHeight(new Vector3(v1.x, 0, v1.y)), v1.y);
            tmp.transform.position = pos;
            tmp.transform.rotation = Quaternion.Euler(0, rnd.Next(0, 360), 0);
            return tmp;
        }
    }
}