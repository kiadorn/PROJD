using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour {

    public GameObject LobbyView;

    public void OpenLobbyView()
    {
        LobbyView.SetActive(true);
    }
}
