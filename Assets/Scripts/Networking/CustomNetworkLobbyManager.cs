using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkLobbyManager : NetworkLobbyManager {

    public override void OnLobbyServerSceneChanged(string sceneName)
    {
        base.OnLobbyServerSceneChanged(sceneName);
        if (sceneName.Equals("LEVEL1") && NetworkServer.connections.Count == maxPlayers)
        {
            Invoke("StartRounds", 2f);
        } else if (sceneName.Equals("Lobby Discovery"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(singleton.gameObject);
        }
        singleton = this;
        DontDestroyOnLoad(gameObject);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void StartRounds()
    {
        RoundManager.instance.CmdStartGame();
    }

    public static void StopClientAndBroadcast()
    {
        CustomNetworkDiscovery.singleton.StopBroadcast();
        onBroadcastStopped += singleton.StopClient;
    }

    public static void StopServerAndBroadcast()
    {
        CustomNetworkDiscovery.singleton.StopBroadcast();
        onBroadcastStopped += singleton.StopServer;
    }

    public static void StopHostAndBroadcast()
    {
        CustomNetworkDiscovery.singleton.StopBroadcast();
        onBroadcastStopped += singleton.StopHost;
    }

    public delegate void Action();
    private static event Action onBroadcastStopped;

    void Update()
    {
        if (onBroadcastStopped != null)
        {
            if (!CustomNetworkDiscovery.singleton.running && CustomNetworkDiscovery.stopConfirmed)
            {
                onBroadcastStopped.Invoke();
                onBroadcastStopped = null;
            }
            else
            {
                if (LogFilter.logDebug)
                    Debug.Log("Waiting for broadcasting to stop completely", gameObject);
                CustomNetworkDiscovery.singleton.StopBroadcast();
            }
        }
    }

}
