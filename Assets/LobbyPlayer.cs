using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public Button ReadyButton;

    private bool shouldIreally = false;

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //DontDestroyOnLoad(gameObject);
        if (shouldIreally)
            transform.SetParent(NetworkLobbyManager.singleton.transform);
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        LobbyList._instance.AddPlayer(this);
        if (isLocalPlayer)
        {
            SetUpLocalPlayer();
        }
        else {
            SetUpOtherPlayer();
        }
        shouldIreally = true;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SetUpLocalPlayer();
    }


    private void SetUpLocalPlayer()
    {
        ReadyButton.interactable = true;
    }

    private void SetUpOtherPlayer()
    {
        ReadyButton.interactable = false;
    }

    public void OnReadyClick()
    {
        CmdShowMeReady();
        SendReadyToBeginMessage();
    }


    [Command]
    public void CmdShowMeReady()
    {
        RpcShowMeReady();
    }

    [ClientRpc]
    public void RpcShowMeReady()
    {
        ReadyButton.image.color = Color.green;
    }







}
