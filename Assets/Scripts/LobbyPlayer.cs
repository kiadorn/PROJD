using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkLobbyPlayer {
    
    [Header("UI")]
    public Button ReadyButton;
    public Image ReadyStatus;
    public Image ReadyIcon;
    public Button BackButton;
    public TMP_InputField playerNameInput;
    public Image background;
    //public TextMeshProUGUI TeamName;
    public Image localIcon;
    public Sprite ReadyIconSprite;
    [SyncVar(hook = "SyncMyBackGround")]
    public Color backgroundColor;

    [SyncVar(hook = "SyncMyTeamText")]
    public string teamText;

    [Header("Persistent Data")]
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
            SetUpBackButton();
        } else
        {
            SetUpOtherPlayer();
        }

    }

    public void OnDestroy()
    {
        print("LobbyPlayer : OnDestroy");
        //if (lobbyPlayerUI)
            //Destroy(lobbyPlayerUI.gameObject);
        if (LobbyList._instance)
            LobbyList._instance.RemovePlayer(this);
    }

    public override void OnClientExitLobby()
    {
        //if (isLocalPlayer && CustomNetworkLobbyManager.singleton.IsClientConnected())
        //CustomNetworkLobbyManager.singleton.client.Disconnect();
        print("LobbyPlayer : OnClientExitLobby");

        lobbyExit.Raise();
        //if (LobbyList._instance)
        //    LobbyList._instance.RemovePlayer(this);
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
        //TeamName.text = teamText;
    }

    private void SetTeamLight()
    {
        background.color = new Color(1, 0.75f, 0);
        Color newColor;
        ColorUtility.TryParseHtmlString("D2C478", out newColor);
        playerNameInput.selectionColor = newColor;
        myTeam = teamLight;
    }

    private void SetTeamShadow()
    {
        background.color = Color.magenta;
        Color newColor;
        ColorUtility.TryParseHtmlString("A677D1", out newColor);
        playerNameInput.selectionColor = newColor;
        myTeam = teamShadow;
    }

    private void SetUpLocalPlayer()
    {
        ReadyButton.gameObject.SetActive(true);
        //ReadyButton.interactable = true;
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

        //ReadyButton.interactable = false;
        //ReadyButton.GetComponent<Image>().enabled = false;
        if (readyToBegin)
        {
            ReadyIcon.sprite = ReadyIconSprite;
            ReadyStatus.color = Color.green;
        }
        ReadyButton.gameObject.SetActive(false);
        playerNameInput.interactable = false;
        playerNameInput.gameObject.GetComponent<Image>().color = Color.red;
        localIcon.gameObject.SetActive(false);
    }

    private void ShowMyName(string newName)
    {
        playerName = newName;
        playerNameInput.text = playerName;
        if (myTeam.TeamName == "Light")
        {
            player1Name.Value = playerName;
        } else if (myTeam.TeamName == "Shadow")
        {
            player2Name.Value = playerName;
        }
    }

    public void OnReadyClick()
    {
        CmdShowMeReady();
        SendReadyToBeginMessage();
        ReadyButton.interactable = false;
        ReadyButton.GetComponent<Image>().color = Color.green;
        SoundManager.instance.PlayPlayerReady();
    }


    [Command]
    public void CmdShowMeReady()
    {
        RpcShowMeReady();
    }

    [ClientRpc]
    public void RpcShowMeReady()
    {
        ReadyIcon.sprite = ReadyIconSprite;
        ReadyStatus.color = Color.green;
        //if(myTeam.TeamName == "Light")
        //{
        //    ReadyButton.image.sprite = LightTeamReadySprite;
        //}
        //else
        //{
        //    ReadyButton.image.sprite = ShadowTeamReadySprite;
        //}
        //ReadyButton.image.color = Color.green;
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
