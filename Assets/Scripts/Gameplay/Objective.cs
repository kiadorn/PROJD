using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Objective : MonoBehaviour {

    private GameObject SM;
    private ObjectiveSpawner _spawner = null;

    private void Start()
    {
        SM = GameObject.Find("SoundManager");
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("OBJECTIVE HIT PLAYER");

            if (_spawner.isServer)
            {
                _spawner.CmdCollidedWithPlayer(other.GetComponent<PlayerController>().myTeamID);
            }
        } else
        {
            Debug.Log("Some other shti");
        }
    }

    public void SetSpawner(ObjectiveSpawner spawner) {
        _spawner = spawner;
    }
}
