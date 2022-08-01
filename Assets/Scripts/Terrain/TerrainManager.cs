using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Terrain;
using UnityEngine.UI;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;
    public RawImage image;
    public static GameGrid gameGrid;
    public float gizmosHeight = 0f;

    public static TerrainManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void initTerrain()
    {
        gameGrid = new GameGrid(100, 100, 5, 5);
        TerrainGenerator terraing = new TerrainGenerator(ref gameGrid, GeneratorSettings.instance.seed);
        terraing.generateTerrain();
        terrain.terrainData.SetHeights(0, 0, terraing.heightmap);
        image.texture = terraing.biomeMapTexture;
    }
    void Start()
    {
        initTerrain();
    }

    private void OnDrawGizmosSelected()
    {
        if (!EditorApplication.isPlaying) return;
        //Vector3 bottomRightCorner = terrain.transform.position + new Vector3(terrain.terrainData.size.x, 0, 0);
        //Vector3 topleftCorner = terrain.transform.position + new Vector3(0, 0, terrain.terrainData.size.z);
        //Vector3 topRightCorner = terrain.transform.position + new Vector3(terrain.terrainData.size.x, 0, terrain.terrainData.size.z);

        //Gizmos.color = Color.red;
        //DrawThickLine(terrain.transform.position, topleftCorner, 10f);
        //DrawThickLine(terrain.transform.position, bottomRightCorner, 10f);
        //DrawThickLine(topRightCorner, topleftCorner, 2f, Color.red);
        //DrawThickLine(topRightCorner, bottomRightCorner, 2f, Color.red);

        //for(int i = 0; i < gameGrid.gridSize.x; i++)
        //{
        //    float xpos = (i*gameGrid.gridSize.x) / (2*gameGrid.chunkGridSize.x);
        //    DrawThickLine(terrain.transform.position+new Vector3(xpos, 0, 0), terrain.transform.position + new Vector3(xpos, 0, terrain.terrainData.size.z), 2f, Color.red);
        //}

        //for (int j = 0; j < gameGrid.gridSize.y; j++)
        //{
        //    float zpos = (j * gameGrid.gridSize.y) / (2 * gameGrid.chunkGridSize.y);
        //    DrawThickLine(terrain.transform.position + new Vector3(0, 0, zpos), terrain.transform.position + new Vector3(terrain.terrainData.size.x, 0, zpos), 2f, Color.red);
        //}

        for (int i = 0; i < gameGrid.gridSize.x; i++)
            for (int j = 0; j < gameGrid.gridSize.y; j++)
            {
                float xpos = (i * gameGrid.chunkGridSize.x) * 2;
                float zpos = (j * gameGrid.chunkGridSize.y) * 2;
                Color wcolor = Color.red;
                if (gameGrid.grid[i, j].biome.biomeName == "Plains") wcolor = Color.green;
                else if (gameGrid.grid[i, j].biome.biomeName == "Water") wcolor = Color.blue;
                Gizmos.color = wcolor;
                Gizmos.DrawLine(new Vector3(xpos, gizmosHeight, zpos), new Vector3(xpos + (gameGrid.chunkGridSize.x * 2), gizmosHeight, zpos + (gameGrid.chunkGridSize.y * 2)));
            }

    }

    private void DrawThickLine(Vector3 start, Vector3 end, float thickness, Color color)
    {
        Handles.DrawBezier(start, end, start, end, color, null, thickness);
    }
}