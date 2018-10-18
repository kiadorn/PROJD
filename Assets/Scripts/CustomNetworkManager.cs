﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public float maxRotationUpdateLimit;

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (NetworkServer.connections.Count == 1)
        {
            Debug.Log("WE 2");
            ServerStatsManager.instance.CmdStartGame();
        }
    }

}
