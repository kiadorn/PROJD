using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class InGamemenueController : MonoBehaviour {

    public bool showGameMenu = false;

    public GameObject inGameManu;
    public GameObject crossHair;

    

	// Use this for initialization
	void Start () {
        inGameManu.SetActive(false);
        //Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (!showGameMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            showGameMenu = true;
            inGameManu.SetActive(true);
            crossHair.SetActive(false);
            SetCursorLock(false);
        }
        else if (showGameMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }      
    
    }

    void SetCursorLock(bool value)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<PlayerController>().mouseLook.SetCursorLock(value);
        }
    }

    public void CloseMenu()
    {
        showGameMenu = false;
        inGameManu.SetActive(false);
        crossHair.SetActive(true);
        SetCursorLock(true);
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
