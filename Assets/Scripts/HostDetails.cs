using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class HostDetails : MonoBehaviour
{
    public string defaultName = "My Game";
    public TMP_InputField gameName;
    public GameObject hostDetails;
    public GameObject LobbyMenu;

    public void DefaultIfEmpty()
    {

        if (String.IsNullOrEmpty(gameName.text))
        {
            gameName.text = defaultName;
        }
    }

    public void ChangeHostView(bool activeStatus)
    {
        hostDetails.SetActive(activeStatus);
        LobbyMenu.SetActive(!activeStatus);
        if (activeStatus)
            gameName.ActivateInputField();
    }

    public void StartDiscoveryBroadcast()
    {
        CustomNetworkDiscovery.singleton.broadcastData = gameName.text;
        CustomNetworkDiscovery.singleton.StartBroadcast();
    }

}
