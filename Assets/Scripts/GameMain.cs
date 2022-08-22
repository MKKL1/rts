using Assets.Scripts.TerrainScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    //Local script
    public List<Entity> entityList = new List<Entity>();
    public static GameMain instance;
    public Terrain mainTerrain;
    public Camera localCamera;
    public SelectionTool localSelectionTool;
    public TerrainManager terrainManager;

    public MainGrid mainGrid;

    private void Awake()
    {
        instance = this;
    }

    public void addEntity(Entity entity)
    {
        entityList.Add(entity);
    }

    public List<Entity> GetEntitiesInBounds(Bounds bounds)
    {
        return entityList.FindAll(x => x.CollidesWith(bounds));
    }
}
