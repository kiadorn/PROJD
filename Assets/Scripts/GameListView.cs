using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameListView : MonoBehaviour {

    public static GameListView instance;

    public GameObject LobbyBar;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }
}
