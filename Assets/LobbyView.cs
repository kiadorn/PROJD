using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour
{

    [Header("TEST")]
    public Image host;
    public Image client;

    public static LobbyView instance;

    public GameObject JoinButton;

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

    private void Start()
    {
        host.gameObject.SetActive(false);
        client.gameObject.SetActive(false);
    }

    public void StartLobby()
    {
        host.gameObject.SetActive(true);
        print("I tried so hard");
    }

    public void JoinLobby() {
        client.gameObject.SetActive(true);
    }

    public void ReadyClient()
    {
        client.color = Color.green;
    }

    public void ReadyHost()
    {
        host.color = Color.green;
    }
}
