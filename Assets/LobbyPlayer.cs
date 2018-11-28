using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LobbyPlayer : NetworkLobbyPlayer {

    public Button ReadyButton;
    public InputField playerNameInput;
    public Image background;
    public TextMeshProUGUI TeamName;
    public Image localIcon;

    public GameEvent lobbyExit;

    [SyncVar(hook = "ShowMyName")]
    public string playerName;

    [SyncVar(hook = "SyncMyBackGround")]
    public Color backgroundColor;

    [SyncVar(hook = "SyncMyTeamText")]
    public string teamText;

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
        base.OnClientEnterLobby();
        if (LobbyList._instance != null)
        {
            LobbyList._instance.AddPlayer(this);

            for (int i = 0; i < LobbyList._instance._players.Count; i++)
            {
                if (i == 0)
                {
                    LobbyList._instance._players[i].SetTeamLight();
                }
                else
                {
                    LobbyList._instance._players[i].SetTeamDark();
                }
            }


            ShowMyName(playerNameInput.text);
        }
        StartCoroutine(WaitForLocalPlayerAuthority());

    }


    private IEnumerator WaitForLocalPlayerAuthority()
    {
        yield return new WaitForEndOfFrame(); //LocalPlayer is not set until end of next frame

        if (isLocalPlayer)
        {
        }

    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        SetUpLocalPlayer();
        //if (isServer)
        //{
        //    SyncMyBackGround(Color.yellow);
        //    SyncMyTeamText("Team Light");
        //}
        //else
        //{
        //    SyncMyBackGround(Color.magenta);
        //    SyncMyTeamText("Team Dark");
        //}
            
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        //SetUpLocalPlayer();
    }

    public override void OnClientExitLobby()
    {
        lobbyExit.Raise();
        base.OnClientExitLobby();
    }

    public void SyncMyBackGround(Color newColor)
    {
        backgroundColor = newColor;
        background.color = backgroundColor;
    }

    public void SyncMyTeamText(string newTeamText)
    {
        teamText = newTeamText;
        TeamName.text = teamText;
    }

    private void SetTeamLight()
    {
        playerNameInput.text = "Mr. Banana Man";
        background.color = new Color(1, 0.75f, 0);
        TeamName.text = "Team Light";
    }

    private void SetTeamDark()
    {
    
        playerNameInput.text = "Wine Guy";
        background.color = Color.magenta;
        TeamName.text = "Team Dark";
    }


    private void SetUpLocalPlayer()
    {
        ReadyButton.interactable = true;
        playerNameInput.interactable = true;
        playerNameInput.gameObject.GetComponent<Image>().color = Color.white;
        ShowMyName(playerNameInput.text);
        localIcon.gameObject.SetActive(true);



    }

    private void SetUpOtherPlayer()
    {
        ReadyButton.interactable = false;
        playerNameInput.interactable = false;
        playerNameInput.gameObject.GetComponent<Image>().color = Color.yellow;
        localIcon.gameObject.SetActive(true);
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
