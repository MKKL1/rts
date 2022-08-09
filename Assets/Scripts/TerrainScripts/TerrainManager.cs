using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.TerrainScripts;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Assets.Scripts.TerrainScripts.BiomeBlending;
using Assets.Scripts.TerrainScripts.Biomes;
using Assets.Scripts.TerrainScripts.Details;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;
    public Transform waterTransform;

    //TODO remove
    public RawImage image;
    public RawImage image2;
    //
    public static TerrainGrid gameGrid;
    public static float waterLevel;
    public static Vector2 terrainCornerBottomLeft;
    public static Vector2 terrainCornerTopRight;
    public float gizmosHeight = 0f;

    public static TerrainManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void initTerrain()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        gameGrid = new TerrainGrid(512, 512, terrain);
        TerrainGenSettings terrainGenSettings = new TerrainGenSettings();
        TerrainGenerator terrainGenerator = new TerrainGenerator(ref gameGrid, terrainGenSettings, GeneratorSettings.instance.seed);
        terrainGenerator.blendingMethod = BlendingMethod.LerpBlending;

        terrainGenerator.GenerateTerrain();

        terrain.terrainData.SetHeights(0, 0, terrainGenerator.heightmap);


        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log(elapsedMs);
        image.texture = terrainGenerator.biomeMapTexture;

        
    }
    void Start()
    {
        
        initTerrain();
        terrainCornerBottomLeft = new Vector2(terrain.transform.position.x, terrain.transform.position.z);
        terrainCornerTopRight = new Vector2(terrainCornerBottomLeft.x + terrain.terrainData.size.x, terrainCornerBottomLeft.y + terrain.terrainData.size.z);
        waterLevel = waterTransform.position.y;
    }

    private void OnDrawGizmosSelected()
    {
        //TODO fix positioning
        if (!EditorApplication.isPlaying) return;

        for (int i = 0; i < gameGrid.gridSize.x; i++)
            for (int j = 0; j < gameGrid.gridSize.y; j++)
            {
                float xpos = i * 2;
                float zpos = j * 2;
                Color wcolor = Color.red;
                if (gameGrid.grid[i, j].biome == BiomeType.PLAINS) wcolor = Color.green;
                else if (gameGrid.grid[i, j].biome == BiomeType.WATER) wcolor = Color.blue;
                Gizmos.color = wcolor;
                Gizmos.DrawLine(new Vector3(xpos, gizmosHeight, zpos), new Vector3(xpos + 2, gizmosHeight, zpos + 2));
            }

    }
}
