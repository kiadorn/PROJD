using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomNetworkManager : NetworkManager {

    [Header("Network Settings")]
    public int amountOfPlayersForStart;
    [Header("MenuButtons")]
    public Button hostButton;
    public Button joinButton;
    public InputField IPAdress;


    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (NetworkServer.connections.Count == amountOfPlayersForStart)
        {
            //Invoke("aaaa", 2f);
        }
    }

    private void aaaa()
    {
        RoundManager.instance.CmdStartGame();
    }

    public void StartHosting()
    {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SetIPAdress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    void SetIPAdress()
    {
        String ipAddress = IPAdress.text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            SetupMenuSceneButtons();
        }
        else
        {
            //SetupOtherSceneButtons();
        }
    }



    void SetupMenuSceneButtons()
    {
        hostButton.onClick.RemoveAllListeners();
        hostButton.onClick.AddListener(StartHosting);

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(JoinGame);


    }

    void SetupOtherSceneButtons()
    {
        GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);

    }

}