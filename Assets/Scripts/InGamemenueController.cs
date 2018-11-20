using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (!showGameMenu&& Input.GetKeyDown("i"))
        {
            showGameMenu = true;
            inGameManu.SetActive(true);
            crossHair.SetActive(false);
            SetCursorLock(false);
        }
        else if (showGameMenu && Input.GetKeyDown("i"))
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
        SceneManager.LoadScene("Lobby");
    }

}
