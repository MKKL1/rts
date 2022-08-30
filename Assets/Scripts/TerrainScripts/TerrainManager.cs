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
using System.Threading.Tasks;

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

    private TerrainGrid terrainGrid;
    private MainGrid mainGrid;

    [Server]
    public void initTerrain()
    {
        mainGrid = GameMain.instance.mainGrid;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        TerrainGenerator terrainGenerator = new TerrainGenerator(257, 257, 4, terrainGenSettings, seed);
        terrainGrid = terrainGenerator.terrainGrid;
        mainGrid = terrainGenerator.CreateMainGrid(257, 257);

        terrainGenerator.GenerateAll(mainGrid);

        

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Debug.Log(elapsedMs);

        SendGridInfoRpc(mainGrid.GetInitialData(), terrainGrid.GetInitialData());

        terrainGrid.IterateChunks(new Action<int, int>((x, y) =>
        {
            SendTerrainChunkRpc(terrainGrid.chunks[x, y], x, y);
        }));

        mainGrid.IterateChunks(new Action<int, int>((x, y) =>
        {
            SendMainChunkRpc(mainGrid.chunks[x, y], x, y);
        }));

        BuildTerrainClient();
        //walkable.material.SetTexture("_Buildable_Mask", GameMain.instance.mainGrid.GetWalkable());
    }

    [ClientRpc(includeOwner = false)]
    public void SendGridInfoRpc(InitialDataMainGrid initmain, InitialDataTerrainGrid initTerr)
    {
        mainGrid = new MainGrid(initmain.gridDataSize.x, initmain.gridDataSize.y, initmain.cellSize.x);
        terrainGrid = new TerrainGrid(initTerr.gridDataSize.x, initTerr.gridDataSize.y, initTerr.cellSize.x, initTerr.cellSize.y);
    }

    [ClientRpc(includeOwner = false)]
    public void SendTerrainChunkRpc(TerrainChunk terrainChunk, int xChunk, int yChunk)
    {
        terrainGrid.chunks[xChunk, yChunk] = terrainChunk;
    }

    [ClientRpc(includeOwner = false)]
    public void SendMainChunkRpc(MainGridChunk mainGridChunk, int xChunk, int yChunk)
    {
        mainGrid.chunks[xChunk, yChunk] = mainGridChunk;
    }

    [ClientRpc]
    private void BuildTerrainClient()
    {

        TerrainBuilder terrainBuilder = new TerrainBuilder(terrainGenSettings, terrain, mainGrid);
        terrainBuilder.BuildTerrain(terrainGrid, detailsTransform);

        terrainCornerBottomLeft = new Vector2(terrain.transform.position.x, terrain.transform.position.z);
        terrainCornerTopRight = terrainCornerBottomLeft + terrainGrid.worldGridSize;

        terrainGenerated?.Invoke();
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
