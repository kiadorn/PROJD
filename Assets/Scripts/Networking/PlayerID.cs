using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour {

    [SyncVar]
    public int playerID;

    public override void OnStartLocalPlayer() {
        CmdSetIdentity();
    }

    [Command]
    private void CmdSetIdentity() {
        RpcSetIdentity();
    }

    [ClientRpc]
    private void RpcSetIdentity() {
        playerID = (int)GetComponent<NetworkIdentity>().netId.Value;
        transform.name = "Player " + playerID;
    }
}
