using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CustomNetworkLobbyManager : NetworkLobbyManager {

    public override void OnLobbyServerSceneChanged(string sceneName)
    {
        base.OnLobbyServerSceneChanged(sceneName);
        if (sceneName.Equals("LEVEL1") && NetworkServer.connections.Count == maxPlayers)
        {
            Invoke("StartRounds", 3f);
        } else if (sceneName.Equals("Lobby Discovery")|| (sceneName.Equals("Main Menu")))
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

    //public override void OnClientSceneChanged(NetworkConnection conn)
    //{
    //    string loadedSceneName = SceneManager.GetSceneAt(0).name;
    //    if (loadedSceneName == lobbyScene)
    //    {
    //        if (client.isConnected)
    //            CallOnClientEnterLobby();
    //    }
    //    else
    //    {
    //        CallOnClientExitLobby();
    //    }
    //    //base.OnClientSceneChanged(conn); //Ger "Connection already ready" error https://forum.unity.com/threads/a-connection-has-already-been-set-as-ready.398832/
    //    OnLobbyClientSceneChanged(conn);
    //}

    void CallOnClientEnterLobby()
    {
        OnLobbyClientEnter();
        foreach (var player in lobbySlots)
        {
            if (player == null)
                continue;

            player.readyToBegin = false;
            player.OnClientEnterLobby();
        }
    }

    void CallOnClientExitLobby()
    {
        OnLobbyClientExit();
        foreach (var player in lobbySlots)
        {
            if (player == null)
                continue;

            player.OnClientExitLobby();
        }
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

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        
        return base.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
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
