using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPlatform : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().mouseLook.SetCursorLock(false);
            CustomNetworkLobbyManager.singleton.StopHost();
            SceneManager.LoadScene("Lobby Discovery");
        }
    }
}
