using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkBehaviour {

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdJoinLobby();
    }

    [Command]
    public void CmdJoinLobby()
    {
        RpcJoinLobby();
    }

    [ClientRpc]
    public void RpcJoinLobby()
    {
        LobbyView.instance.JoinLobby();
    }

    [Command]
    public void CmdReadyUp()
    {
        RpcReadyUp();
    }

    [ClientRpc]
    public void RpcReadyUp()
    {
        LobbyView.instance.ReadyClient();
    }
}
