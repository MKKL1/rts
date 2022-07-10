using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class ServerScript : NetworkBehaviour
{
    //TODO playerlist has to be sent to clients only when player leaves or joins, use RPCclient ?
    //[SyncVar(hook = nameof(onPlayerListChange))]
    



    //[ServerCallback]

    //TODO better way to call this function
    //private void onPlayerListChange(List<PlayerScript> oldList, List<PlayerScript> newList)
    //{
    //    playerList = newList;
    //    playerListChangeEvent.Invoke();
    //}
}
