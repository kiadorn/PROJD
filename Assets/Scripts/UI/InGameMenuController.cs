using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class InGameMenuController : MenuController {

    private bool showGameMenu = false;
    [SerializeField]
    private GameObject inGameMenu;
    [SerializeField]
    private GameObject crossHair;

	void Start () {
        SetMenuState(false);
        SwitchViewTo(views[0]);
    }
	
	void Update () {
        if (!showGameMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenuState(true);
            SwitchViewTo(views[0]);
        }
        else if (showGameMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenuState(false);
        }      
    }

    void SetCursorLock(bool @lock)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<PlayerController>().mouseLook.SetCursorLock(@lock);
        }
    }

    public void SetMenuState(bool open)
    {
        showGameMenu = open;
        inGameMenu.SetActive(open);
        crossHair.SetActive(!open);
        SetCursorLock(!open);
    }

    public void LeaveMatch()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                if (player.GetComponent<NetworkIdentity>().isServer)
                {
                    CustomNetworkLobbyManager.singleton.StopHost();
                } else
                {
                    CustomNetworkLobbyManager.singleton.StopClient();
                }
            } 
        }
        SceneManager.LoadScene("Lobby Discovery");
    }
}
