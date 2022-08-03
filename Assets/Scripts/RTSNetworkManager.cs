using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public enum GameState
{
    LOBBY,
    GAME_ACTIVE
}

public class RTSNetworkManager : NetworkManager
{
    public List<PlayerScript> playerList = new List<PlayerScript>();
    public UnityEvent playerListChangeEvent = new UnityEvent();
    public GameState gameState = GameState.LOBBY;

    [Scene]
    public string gameScene;


    public static RTSNetworkManager instance;
    public override void Awake()
    {
        base.Awake();
        instance = this;
    }
    public void AddPlayer(PlayerScript playerScript)
    {
        playerList.Add(playerScript);
        playerListChangeEvent?.Invoke();
    }


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerScript playerScript = conn.identity.GetComponent<PlayerScript>();
        var data = new InitialPlayerData();
        data.name = $"Player {conn.connectionId}";
        switch(gameState)
        {
            case GameState.LOBBY:
                data.state = PlayerState.IN_ROOM;
                break;

            case GameState.GAME_ACTIVE:
                data.state = PlayerState.OBSERVING;
                break;
        }

        playerScript.InitPlayer(data);
        Debug.Log("OnServerAddPlayer" + data.name);
    }

    
    public void startGame(bool changescene)
    {
        if (gameState != GameState.LOBBY) return;

        if (changescene) ServerChangeScene(gameScene);
        playerList.ForEach(x => x.state = PlayerState.PLAYING);
        gameState = GameState.GAME_ACTIVE;
    }



    public override void OnStartServer()
    {
        Debug.Log("Server start");
    }

    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    base.OnServerAddPlayer(conn);
    //    ServerScript.instance.updatePlayerList();
    //}
    //Serverside
    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    Transform startPos = GetStartPosition();
    //    GameObject player = startPos != null
    //        ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
    //        : Instantiate(playerPrefab);

    //    player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
    //    string playername = $"Player {conn.connectionId}";

    //    player.GetComponent<PlayerScript>().playerName = playername;
    //    Debug.Log("added ? ");

    //    NetworkServer.AddPlayerForConnection(conn, player);
    //}
}
