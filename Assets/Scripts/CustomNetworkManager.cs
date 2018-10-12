using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    private void Update()
    {
        if (NetworkServer.connections.Count == 2)
        {
            Debug.Log("WE 2");
        }
    }

}
