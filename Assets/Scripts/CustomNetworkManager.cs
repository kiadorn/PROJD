using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (NetworkServer.connections.Count == 2)
        {
            Debug.Log("WE 2");
            ServerStatsManager.instance.CmdStartGame();
        }

    }



}
