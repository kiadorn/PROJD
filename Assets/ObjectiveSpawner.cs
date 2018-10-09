using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawner : MonoBehaviour {

    private Objective _ball;
    public int Spawntime;
    public bool StartWithBall;
    private float _currentSpawntime;
    private bool _spawning;

    public void SetObjectiveOnCooldown() {
        _spawning = true;
    }

    void Start() {
        _ball = GetComponentInChildren<Objective>();
        _ball.SetSpawner(this);
        _spawning = !StartWithBall;
        _currentSpawntime = Spawntime;
        if (!StartWithBall) {
            _ball.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update () {
        if (_spawning) {
            _currentSpawntime -= Time.deltaTime;
            if (_currentSpawntime < 0) {
                EnableObjective();
                _spawning = false;
                _currentSpawntime = Spawntime;
            }
        }
	}

    private void EnableObjective() {
        _ball.gameObject.SetActive(true);
    }
}
