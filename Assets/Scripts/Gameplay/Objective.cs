using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Objective : MonoBehaviour {

    private ObjectiveSpawner _spawner = null;

    public void CollectObjective(int teamID) {
        _spawner.OrbGet(teamID);
    }

    //void OnTriggerEnter(Collider other) {
    //    if (other.CompareTag("Player")) {
    //        _spawner.OrbGet(other.GetComponent<PlayerController>().myTeamID);
    //    } 
    //}

    public void SetSpawner(ObjectiveSpawner spawner) {
        _spawner = spawner;
    }
}
