using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour {

    [SyncVar]
    public int playerID;

    public override void OnStartLocalPlayer()
    {
        CmdSetIdentity();
        CmdAddToPlayerList();
        if (CustomNetworkManager.singleton.numPlayers == 2)
        {
            CmdStartGame();
        }
    }

    [Command]
    private void CmdStartGame()
    {
        ServerStatsManager.instance.RpcStartGame();
    }


    [Command]
    private void CmdSetIdentity()
    {
        RpcSetIdentity();
    }

    [ClientRpc]
    private void RpcSetIdentity()
    {
        playerID = (int)GetComponent<NetworkIdentity>().netId.Value;
        transform.name = "Player " + playerID;
        Debug.Log("This player's ID is " + playerID);
    }

    [Command]
    private void CmdAddToPlayerList()
    {
        RpcAddToPlayerList();
    }

    [ClientRpc]
    private void RpcAddToPlayerList()
    {
        ServerStatsManager.instance.playerList.Add(transform.gameObject);
    }
}
