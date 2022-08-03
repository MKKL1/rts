using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum PlayerState
{
    IN_ROOM,
    PLAYING,
    OBSERVING
}

public struct InitialPlayerData
{
    public string name;
    public PlayerState state;
}

public class PlayerScript : NetworkBehaviour
{
    //public readonly static List<PlayerScript> playerList = new List<PlayerScript>();

    public Vector3 playerPosition;

    [SyncVar(hook = nameof(onPlayerNameChange))]
    public string playerName;

    [SyncVar]
    public PlayerState state;

    //Run by server
    public void InitPlayer(InitialPlayerData playerData)
    {
        Debug.Log("INIT PLAYER " + playerData.name);
        playerName = playerData.name;
        
    }

    //public override void OnStartServer()
    //{
    //    base.OnStartServer();
        

    //}

    public override void OnStartClient()
    {
        Debug.Log("OnStartClient" + playerName);
        base.OnStartClient();
        RTSNetworkManager.instance.AddPlayer(this);
    }

    public void onPlayerNameChange(string oldname, string newname)
    {
        RTSNetworkManager.instance.playerListChangeEvent?.Invoke();
    }

    //DUPA DUPA DUPAAAAAAAAAAAAAAA DUPA DUPACZKA DUPA DUPA DUPA CHUJ CIPA CYCE WADOWICE JEBANE RUCHANIE
    //public override void OnStartServer()
    //{
    //    Debug.Log("START SERVER");
    //    base.OnStartServer();

    //    Debug.Log("HERE" + UIManager.instance.selectionTool);

    //}

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    playerName = $"Player {connectionToClient.connectionId}";

    //}

    //public override void OnStartLocalPlayer()
    //{
    //    //UIManager.instance.updatePlayerList();
    //}

    //private void Start()
    //{
    //    Debug.Log("START NORMAL");

    //    RTSNetworkManager.instance.addPlayer(this);
    //    //if(isLocalPlayer)
    //    //    UIManager.instance.updatePlayerList();
    //}

    public override void OnStopServer()
    {
        RTSNetworkManager.instance.playerList.Remove(this);
    }
}

