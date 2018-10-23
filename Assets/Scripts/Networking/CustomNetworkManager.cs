using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public int amountOfPlayersForStart;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (NetworkServer.connections.Count == amountOfPlayersForStart)
        {
            ServerStatsManager.instance.CmdStartGame();
        }
    }
}
