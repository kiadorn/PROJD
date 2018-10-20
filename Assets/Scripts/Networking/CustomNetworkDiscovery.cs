using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkDiscovery : NetworkDiscovery {

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("Found shit");
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
    }

    
}
