using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMenuManager : NetworkBehaviour
{
    public bool debug = false;
    public static NetworkMenuManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Command(requiresAuthority = false)]
    public void cmdStartGame()
    {
        //TODO if all players ready
        RTSNetworkManager.instance.startGame(!debug);
    }
}
