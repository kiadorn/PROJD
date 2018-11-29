using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkLobbyPlayer {
    
    //UI
    public Button ReadyButton;
    public Button BackButton;
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

    public LobbyPlayerUI lobbyPlayerUI;

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
                    LobbyList._instance._players[i].SetTeamShadow();
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
            //SetUpBackButton();
        } else
        {
            SetUpOtherPlayer();
        }
    }

    public void OnDestroy()
    {
        if (lobbyPlayerUI)
            Destroy(lobbyPlayerUI.gameObject);
    }

    public override void OnClientExitLobby()
    {
        //if (isLocalPlayer && CustomNetworkLobbyManager.singleton.IsClientConnected())
        //CustomNetworkLobbyManager.singleton.client.Disconnect();
        Debug.LogFormat("LobbyPlayer {0} : OnClientExitLobby", GetComponent<NetworkIdentity>() ? GetComponent<NetworkIdentity>().netId.ToString() : "NO_ID");

        lobbyExit.Raise();
        LobbyList._instance.RemovePlayer(this);
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

    private void SetTeamShadow()
    {
    
        playerNameInput.text = "Wine Guy";
        background.color = Color.magenta;
        TeamName.text = "Team Shadow";
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

    public void SetUpBackButton()
    {
        BackButton = GameObject.Find("BackFromLobbyButton").GetComponent<Button>();
        if (isServer)
        {
#if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.AddPersistentListener(BackButton.onClick, new UnityAction(AddStopHostListener));
#endif
            BackButton.onClick.AddListener(AddStopHostListener);
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.AddPersistentListener(BackButton.onClick, new UnityAction(AddStopClientListener));
#endif
            BackButton.onClick.AddListener(AddStopClientListener);
        }
#if UNITY_EDITOR
        //UnityEditor.Events.UnityEventTools.AddPersistentListener(BackButton.onClick, new UnityAction(AddDiscoveryListening));
#endif
        //BackButton.onClick.AddListener(AddDiscoveryListening);
        //AT SOME POINT, CLEAR THIS LIST OF LISTENERS (OR PERHAPS SCENE CHANGE DOES IT FOR US)
    }

    private void AddStopHostListener()
    {
        CustomNetworkLobbyManager.StopHostAndBroadcast();
        CustomNetworkLobbyManager.singleton.client.Disconnect();
        SceneManager.LoadScene(0);
    }

    private void AddStopClientListener()
    {
        CustomNetworkLobbyManager.StopClientAndBroadcast();
        CustomNetworkLobbyManager.singleton.client.Disconnect();
        SceneManager.LoadScene(0);
    }

    private void AddDiscoveryListening()
    {
        CustomNetworkDiscovery.singleton.StartListening();
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
        } else if (myTeam.teamName == "Shadow")
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
