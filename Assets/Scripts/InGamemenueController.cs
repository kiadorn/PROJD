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
        Cursor.visible = false;
        

    }
	
	// Update is called once per frame
	void Update () {

        if (!showGameMenu&& Input.GetKeyDown("i"))
        {
            showGameMenu = true;
            inGameManu.SetActive(true);
            Cursor.visible = true;
            crossHair.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
        else if (showGameMenu && Input.GetKeyDown("i"))
        {
            CloseMenu();
        }      
    
    }

    public void CloseMenu()
    {
        showGameMenu = false;
        inGameManu.SetActive(false);
        Cursor.visible = false;
        crossHair.SetActive(true);
    }

    public void LeaveMatch()
    {
        SceneManager.LoadScene("Lobby");
    }

}
