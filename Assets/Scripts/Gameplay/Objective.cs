using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class Objective : NetworkBehaviour {

    private GameObject SM;
    private ObjectiveSpawner _spawner = null;

    private void Start()
    {
        SM = GameObject.Find("SoundManager");
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("OBJECTIVE HIT PLAYER");

            if (isServer)
            {
                CmdCollidedWithPlayer(other.GetComponent<PlayerController>().myTeamID);
            }
        } else
        {
            Debug.Log("Some other shti");
        }
    }

    [Command]
    public void CmdCollidedWithPlayer(int teamID)
    {
        RpcCollidedWithPlayer(teamID);
        
    }

    [ClientRpc]
    public void RpcCollidedWithPlayer(int teamID)
    {
        SM.GetComponent<SoundManager>().PlayAllyPoint(transform.parent.GetChild(1).gameObject);
        ServerStatsManager.instance.AddPoint(teamID);
        _spawner.SetObjectiveOnCooldown();
        gameObject.SetActive(false);
    }

    public void SetSpawner(ObjectiveSpawner spawner) {
        _spawner = spawner;
    }
}
