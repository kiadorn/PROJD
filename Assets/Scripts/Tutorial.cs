using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tutorial : NetworkBehaviour {

    private bool ready = false;
    private int playersReady = 0;
	
	void Update () {
        CheckIfReady();
	}

    void CheckIfReady()
    {
        if (Input.GetKey(KeyCode.R) && !ready)
        {
            ready = true;
            CmdMeReady();
        }
        if (isServer)
        {
            if (playersReady == 2)
            {
                ServerStatsManager.instance.CmdStartGame();
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
