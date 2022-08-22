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
using Assets.Scripts.TerrainScripts.Generation.Noise;
using Mirror;
using Assets.Scripts.TerrainScripts.Generation;
using System;

public class TerrainManager : NetworkBehaviour
{
    public Terrain terrain;
    public Transform waterTransform;
    public Transform detailsTransform;
    public TerrainGenSettings terrainGenSettings;

    //TODO remove
    public MeshRenderer walkable;

    public float waterLevel;
    public Vector2 terrainCornerBottomLeft;
    public Vector2 terrainCornerTopRight = new Vector2(500, 500);
    public float gizmosHeight = 0f;

    public event Action terrainGenerated;

    public int seed = 69;

    public static TerrainManager instance;
    private void Awake()
    {
        instance = this;
    }

    [Server]
    public void initTerrain()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        TerrainGenerator terrainGenerator = new TerrainGenerator(257, 257, 4, terrainGenSettings, seed);
        GameMain.instance.mainGrid = terrainGenerator.CreateMainGrid(257, 257);
        terrainGenerator.blendingMethod = BlendingMethod.LerpBlending; 

        //TODO use compression
        TerrainGeneratorResult msg = terrainGenerator.Generate();
        BuildTerrainClient(msg);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log(elapsedMs);

        walkable.material.SetTexture("_Buildable_Mask", GameMain.instance.mainGrid.GetWalkable());
    }

    [ClientRpc]
    private void BuildTerrainClient(TerrainGeneratorResult msg)
    {

        GameMain.instance.mainGrid = new MainGrid(msg.mainGridSize, msg.terrainSize);
        TerrainBuilder terrainBuilder = new TerrainBuilder(terrainGenSettings, terrain, GameMain.instance.mainGrid);
        terrainBuilder.BuildTerrain(msg, detailsTransform);

        terrainCornerBottomLeft = new Vector2(terrain.transform.position.x, terrain.transform.position.z);
        terrainCornerTopRight = terrainCornerBottomLeft + msg.terrainSize;

        terrainGenerated.Invoke();
    }

    void Start()
    {
        
        waterLevel = waterTransform.position.y;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    //TODO fix positioning
    //    if (!EditorApplication.isPlaying) return;

    //    for (int i = 0; i < terrainGrid.gridSize.x; i++)
    //        for (int j = 0; j < terrainGrid.gridSize.y; j++)
    //        {
    //            float xpos = i * terrainGrid.cellSize.x;
    //            float zpos = j * terrainGrid.cellSize.y;
    //            Color wcolor = Color.red;
    //            if (terrainGrid.grid[i, j].biome == BiomeType.PLAINS) wcolor = Color.green;
    //            else if (terrainGrid.grid[i, j].biome == BiomeType.WATER) wcolor = Color.blue;
    //            else if (terrainGrid.grid[i, j].biome == BiomeType.MOUNTAINS) wcolor = Color.gray;
    //            Gizmos.color = wcolor;
    //            Gizmos.DrawLine(new Vector3(xpos, gizmosHeight, zpos), new Vector3(xpos + terrainGrid.cellSize.x, gizmosHeight, zpos + terrainGrid.cellSize.y));
    //        }

    //}
}
