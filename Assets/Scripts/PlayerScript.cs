using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Assets.Scripts.Networking;

public enum PlayerState
{
    IN_ROOM,
    PLAYING,
    OBSERVING
}

public struct InitialPlayerData
{
    public PlayerIdentificator id;
    public string name;
    public PlayerState state;
}

public class PlayerScript : NetworkBehaviour
{
    //public readonly static List<PlayerScript> playerList = new List<PlayerScript>();
    [SyncVar]
    public PlayerIdentificator playerID;
    public Vector3 playerPosition;

    [SyncVar]
    public string playerName = "default";

    [SyncVar]
    public PlayerState state;

    public void SetPlayerData(InitialPlayerData data)
    {
        playerID = data.id;
        playerName = data.name;
        state = data.state;
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            GameMain.instance.localPlayerScript = this;
            GameMain.instance.localPlayerID = playerID;
        }
        GameMain.instance.AddPlayer(this);
    }

    public override void OnStopServer()
    {
        GameMain.instance.RemovePlayer(this);
    }
}

