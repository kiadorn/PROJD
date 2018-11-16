using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyManager : NetworkLobbyManager {

    public GameObject lobbyList;
    public GameObject lobbyBar;

	public void StartNewServer()
    {
        StartServer();
        Instantiate(lobbyBar, lobbyList.transform);
    }

    public void JoinServer()
    {
        StartClient();

    }
}
