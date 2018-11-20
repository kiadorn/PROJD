using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    public Button ReadyButton;
    public InputField playerNameInput;

    [SyncVar(hook = "ShowMyName")]
    public string playerName;


    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        //DontDestroyOnLoad(gameObject);
        transform.SetParent(NetworkLobbyManager.singleton.transform);
    }

    public override void OnStartLocalPlayer()
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

        ShowMyName(playerNameInput.text);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        //SetUpLocalPlayer();
    }


    private void SetUpLocalPlayer()
    {
        ReadyButton.interactable = true;
        ShowMyName(playerNameInput.text);
        playerNameInput.interactable = true;
    }

    private void SetUpOtherPlayer()
    {
        ReadyButton.interactable = false;
        playerNameInput.interactable = false;
    }

    private void ShowMyName(string newName)
    {
        playerName = newName;
        playerNameInput.text = playerName;
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

    public void UpdateName()
    {
        CmdNameChanged(playerNameInput.text);
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
    }





}
