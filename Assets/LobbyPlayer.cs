using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public Button ReadyButton;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        DontDestroyOnLoad(gameObject);
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


    [Command]
    public void CmdJoinLobby()
    {
        RpcJoinLobby();
    }

    [ClientRpc]
    public void RpcJoinLobby()
    {
        LobbyView.instance.JoinLobby();
    }

    [Command]
    public void CmdReadyUp()
    {
        RpcReadyUp();
    }

    [ClientRpc]
    public void RpcReadyUp()
    {
        LobbyView.instance.ReadyClient();
    }


}
