using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyManager : NetworkLobbyManager {

    public GameObject menuView;
    public GameObject hostView;
    public GameObject findView;
       

	public void StartNewServer()
    {
        menuView.SetActive(false);
        hostView.SetActive(true);
        StartServer();
    }

    public void JoinServer()
    {
        menuView.SetActive(false);
        findView.SetActive(true);
        StartClient();
    }
}
