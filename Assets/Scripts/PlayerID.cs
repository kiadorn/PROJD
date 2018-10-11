using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour {

    [SyncVar]
    public int playerID;

    public override void OnStartLocalPlayer()
    {
        SetIdentity();

    }

    private void SetIdentity()
    {
        playerID = (int)GetComponent<NetworkIdentity>().netId.Value;
        transform.name = "Player " + playerID;
        Debug.Log("This player's ID is " + playerID);
    }
}
