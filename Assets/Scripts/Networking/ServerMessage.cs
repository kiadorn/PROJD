using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerMessage : NetworkBehaviour {

    public Text shit;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            RpcDoShit();
    }

    [ClientRpc]
    public void RpcDoShit()
    {
        shit.text = "You did get this even though it's Server Only WOW!";
    }
}
