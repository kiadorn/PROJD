using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class ObjectiveSpawner : NetworkBehaviour {

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

    public void OrbGet(int teamID)
    {
        //CmdCollidedWithPlayer(teamID);
    }

    public void Spawn() {

    }

    public void Despawn()
    {

    }

    public IEnumerator SpawnTimer(float spawnTimer) {

        //Play sound!
        //Cool effect
        //yield return new WaitForSeconds(spawnTimer);
        //Remove Cool effect

        for (float i = 0; i < spawnTimer; i+=Time.deltaTime)
        {
            print("Spawntimer: " + i.ToString());


            yield return 0;
        }
        _ball.gameObject.SetActive(true);

        yield return 0;
    } 

    [ClientRpc]
    private void RpcDespawnOrb()
    {
        //stoppa ljud? Idé - hitta ljudkälla och ta bort objektet den ligger på.
        _ball.gameObject.SetActive(false);
    }

    [Command]
    public void CmdCollidedWithPlayer(int teamID) {
        RpcCollidedWithPlayer(teamID);

    }

    [ClientRpc]
    public void RpcCollidedWithPlayer(int teamID) {
        SoundManager.instance.PlayAllyPoint(transform.GetChild(1).gameObject);
        ServerStatsManager.instance.AddPoint(teamID);
        SetObjectiveOnCooldown();
        _ball.gameObject.SetActive(false);
    }
}
