using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;
    void Start()
    {
        Debug.Log("AWAKE2");
        TerrainGenerator terraing = new TerrainGenerator(300, 300, 51);
        terraing.generateTerrain();
        terrain.terrainData.SetHeights(0, 0, terraing.heightmap);
    }
}
