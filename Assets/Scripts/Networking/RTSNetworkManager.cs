using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using System;

public enum GameState
{
    LOBBY,
    GAME_ACTIVE
}

public class RTSNetworkManager : NetworkManager
{
    
    
    public GameState gameState = GameState.LOBBY;

    [Scene]
    public string gameScene;

    private byte playerIterator = 1;


    public static RTSNetworkManager instance;
    public override void Awake()
    {
        base.Awake();
        instance = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        //Setting player data before sending it to clients
        PlayerScript playerScript = player.GetComponent<PlayerScript>();
        var data = new InitialPlayerData();
        data.name = $"Player {conn.connectionId}";
        data.id = playerIterator;
        playerIterator++;
        switch (gameState)
        {
            case GameState.LOBBY:
                data.state = PlayerState.IN_ROOM;
                break;

            case GameState.GAME_ACTIVE:
                data.state = PlayerState.OBSERVING;
                break;
        }
        playerScript.SetPlayerData(data);


        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        GameMain.instance.entityManager.visibleEntityList.ForEach(x => x.transform.GetComponent<NetworkIdentity>().AssignClientAuthority(conn));
    }

    [Server]
    public void startGame(bool changescene)
    {
        if (gameState != GameState.LOBBY) return;

        if (changescene) ServerChangeScene(gameScene);
        foreach (var player in GameMain.instance.playerList)
        {
            player.state = PlayerState.PLAYING;
        }
        gameState = GameState.GAME_ACTIVE;

        GameMain.instance.terrainManager.initTerrain();
    }



    public override void OnStartServer()
    {
        Debug.Log("Server start");
    }
}
