using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (NetworkServer.connections.Count == 2)
        {
            Debug.Log("WE 2");
            ServerStatsManager.instance.CmdStartGame();
        }
    }

}
