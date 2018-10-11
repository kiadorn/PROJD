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
        CmdAddToPlayerList();
    }

    //Görs inte via server
    private void SetIdentity()
    {
        playerID = (int)GetComponent<NetworkIdentity>().netId.Value;
        transform.name = "Player " + playerID;
        Debug.Log("This player's ID is " + playerID);
    }

    [Command]
    private void CmdAddToPlayerList()
    {
        ServerStatsManager.instance.playerList.Add(transform.gameObject);
        RpcAddToPlayerList();
    }

    [ClientRpc]
    private void RpcAddToPlayerList()
    {
        ServerStatsManager.instance.playerList.Add(transform.gameObject);
    }
}
