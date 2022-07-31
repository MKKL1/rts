using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets;
using UnityEngine.UI;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;
    public RawImage image;
    public static GameGrid grid;

    public static TerrainManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void initTerrain()
    {
        grid = new GameGrid(100, 100, 5, 5);
        TerrainGenerator terraing = new TerrainGenerator(grid, 51);
        terraing.generateTerrain();
        terrain.terrainData.SetHeights(0, 0, terraing.heightmap);
        image.texture = terraing.biomeMapTexture;
    }
    void Start()
    {
        initTerrain();
    }
}
