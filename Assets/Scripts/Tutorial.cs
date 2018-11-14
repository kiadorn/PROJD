using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tutorial : NetworkBehaviour {

    private bool ready = false;
    [ReadOnly]
    public int playersReady = 0;
    public int playersToStart;
	
	void Update () {
        CheckIfReady();
	}

    void CheckIfReady()
    {
        if (isLocalPlayer && Input.GetKey(KeyCode.R) && !ready)
        {
            ready = true;
            CmdMeReady();
            print("CMD ME");
                
        }
        if (isServer)
        {
            if (playersReady == playersToStart)
            {
                RoundManager.instance.CmdStartGame();
                playersReady = 0;
            }
        }
    }

    [Command]
    void CmdMeReady()
    {
        playersReady++;
    }
}
