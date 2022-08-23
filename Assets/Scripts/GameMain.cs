using Assets.Scripts.TerrainScripts;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : NetworkBehaviour
{
    public List<PlayerScript> playerList = new List<PlayerScript>();
    public event Action playerListChangeEvent;

    //Synchronized by spawning and despawning entities
    public List<Entity> entityList = new List<Entity>();
    
    public Terrain mainTerrain;
    public Camera localCamera;
    public SelectionTool localSelectionTool;
    public TerrainManager terrainManager;

    public MainGrid mainGrid;

    public static GameMain instance;

    private void Awake()
    {
        instance = this;
    }

    public void addEntity(Entity entity)
    {
        entityList.Add(entity);
    }

    public void AddPlayer(PlayerScript playerScript)
    {
        playerList.Add(playerScript);
        playerListChangeEvent?.Invoke();
    }

    public void RemovePlayer(PlayerScript playerScript)
    {
        playerList.Remove(playerScript);
        playerListChangeEvent?.Invoke();
    }

    public List<Entity> GetEntitiesInBounds(Bounds bounds)
    {
        return entityList.FindAll(x => x.CollidesWith(bounds));
    }
}
