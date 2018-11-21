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
    }
}