using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LobbyPlayer : NetworkLobbyPlayer {
    
    //UI
    public Button ReadyButton;
    public InputField playerNameInput;
    public Image background;
    public TextMeshProUGUI TeamName;
    public Image localIcon;
    [SyncVar(hook = "SyncMyBackGround")]
    public Color backgroundColor;

    [SyncVar(hook = "SyncMyTeamText")]
    public string teamText;



    //Persistent Data
    public TeamAsset teamLight;
    public TeamAsset teamShadow;
    public StringVariable player1Name;
    public StringVariable player2Name;
    public TeamAsset myTeam;
    public GameEvent lobbyExit;
    [SyncVar(hook = "ShowMyName")]
    public string playerName;

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
            SetUpLocalPlayer();
        } else
        {
            SetUpOtherPlayer();
        }
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
        playerNameInput.text = "Mr. Cheeto Man";
        background.color = new Color(1, 0.75f, 0);
        TeamName.text = "Team Light";
        myTeam = teamLight;
    }

    private void SetTeamDark()
    {
    
        playerNameInput.text = "Wine Guy";
        background.color = Color.magenta;
        TeamName.text = "Team Dark";
        myTeam = teamShadow;
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
        playerNameInput.gameObject.GetComponent<Image>().color = Color.red;
        localIcon.gameObject.SetActive(false);
    }

    private void ShowMyName(string newName)
    {
        playerName = newName;
        playerNameInput.text = playerName;
        if (myTeam.teamName == "Light")
        {
            player1Name.Value = playerName;
        } else if (myTeam.teamName == "Dark")
        {
            player2Name.Value = playerName;
        }
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
