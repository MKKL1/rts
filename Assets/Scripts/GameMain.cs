using Assets.Scripts.Networking;
using Assets.Scripts.Simulation;
using Assets.Scripts.TerrainScripts;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public PlayerIdentificator localPlayerID = PlayerIdentificator.serverID;
    public List<PlayerScript> playerList = new List<PlayerScript>();
    public event Action playerListChangeEvent;
    public EntityManager entityManager = new EntityManager();


    public PlayerScript localPlayerScript;
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

    
}
