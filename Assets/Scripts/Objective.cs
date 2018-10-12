using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Objective : NetworkBehaviour {

    private ObjectiveSpawner _spawner = null;

	void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            //other.GetComponent<PlayerStats>().AddPoint();
            _spawner.SetObjectiveOnCooldown();
            this.gameObject.SetActive(false);
        }
    }

    public void SetSpawner(ObjectiveSpawner spawner) {
        _spawner = spawner;
    }
}
