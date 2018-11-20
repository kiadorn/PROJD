using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public Button ReadyButton;

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //DontDestroyOnLoad(gameObject);
        transform.SetParent(NetworkLobbyManager.singleton.transform);
    }

    public override void OnClientEnterLobby()
    {
        LobbyList._instance.AddPlayer(this);
        base.OnClientEnterLobby();
        //this.transform.SetParent(null);
        if (isLocalPlayer)
        {
            SetUpLocalPlayer();
        }
            //CmdJoinLobby();
        else {
            SetUpOtherPlayer();
        }
    }


    private void SetUpLocalPlayer()
    {

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
