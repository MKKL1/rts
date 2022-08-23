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

    [SyncVar]
    public string playerName = "default";

    [SyncVar]
    public PlayerState state;

    public void SetPlayerData(InitialPlayerData data)
    {
        playerName = data.name;
        state = data.state;
    }

    public override void OnStartClient()
    {
        GameMain.instance.AddPlayer(this);
    }

    public override void OnStopServer()
    {
        GameMain.instance.RemovePlayer(this);
    }
}

