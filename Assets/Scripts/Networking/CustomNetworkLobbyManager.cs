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

    public override void OnLobbyServerPlayersReady()
    {
        print("I tried so HAAARD");
        foreach (LobbyPlayer lobbyPlayer in LobbyList._instance._players)
        {
            if (lobbyPlayer.isLocalPlayer)
            {
                lobbyPlayer.transform.SetParent(null);
                DontDestroyOnLoad(lobbyPlayer.gameObject);
            }
        }
        base.OnLobbyServerPlayersReady();
    }

    //public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    //{
    //    GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

    //    return obj;
    //}

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
