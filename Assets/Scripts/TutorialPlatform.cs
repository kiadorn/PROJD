using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPlatform : MonoBehaviour {

    private void Start()
    {
        if (RoundManager.instance)
            RoundManager.instance.tutorialActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().mouseLook.SetCursorLock(false);
            CustomNetworkLobbyManager.singleton.StopHost();
            SceneManager.LoadScene("Main Menu");
        }
    }
}
