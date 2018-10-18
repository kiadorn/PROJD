using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class Objective : NetworkBehaviour {

    private ObjectiveSpawner _spawner = null;

	void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("OBJECTIVE HIT PLAYER");

            if (isServer)
            {
                CmdCollidedWithPlayer(other.GetComponent<RigidbodyFirstPersonController>().myTeamID);
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
        ServerStatsManager.instance.AddPoint(teamID);
        _spawner.SetObjectiveOnCooldown();
        gameObject.SetActive(false);
    }

    public void SetSpawner(ObjectiveSpawner spawner) {
        _spawner = spawner;
    }
}
