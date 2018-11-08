using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class ObjectiveSpawner : NetworkBehaviour {

    public int delayBetweenSpawns;
    public bool StartWithBall;
    public int pointValueTeamWhite;
    public int pointValueTeamBlack;
    public bool independantSpawn;
    private GameObject _ball;
    //private bool _spawning;
    private ParticleSystem _respawnParticles;

    public void SetObjectiveOnCooldown() {
        //_spawning = true;
    }

    void Start() {
        _ball = transform.Find("Ball").gameObject;
        //_spawning = !StartWithBall;
        _respawnParticles = transform.Find("RespawnParticles").GetComponent<ParticleSystem>();
    }

    public void CollectObjective(int teamID)
    {
        int amountOfPoints = 0;
        if (teamID == 1)
        {
            amountOfPoints = pointValueTeamWhite;
        } else
        {
            amountOfPoints = pointValueTeamBlack;
        }

        SoundManager.instance.PlayAllyPoint(gameObject);
        ServerStatsManager.instance.AddPoint(teamID, amountOfPoints);

        Despawn();

        if (!independantSpawn)
        {
            ObjectiveSpawnManager.instance.Despawn(this);
            
            ObjectiveSpawnManager.instance.SpawnNext();
        } else
        {
            ObjectiveSpawnManager.instance.SpawnMe(this);
        }
    }
    
    public void StartRespawn()
    {
        StartCoroutine(SpawnTimer(ObjectiveSpawnManager.instance.spawnTimer)); //Respawn effekter
        SoundManager.instance.PlayOrbRespawn(gameObject);
    }

    public void Spawn() {
        _ball.SetActive(true); //Spawn effekter
        print("helo");
        if (_ball.transform.childCount == 0)
            SoundManager.instance.PlayOrbSound(_ball);


    }

    public void Despawn()
    {
        _ball.SetActive(false); //Despawn effekter
    }

    public IEnumerator SpawnTimer(float spawnTimer) {

        //Cool effect
        //yield return new WaitForSeconds(spawnTimer);
        //Remove Cool effect

        _respawnParticles.gameObject.SetActive(true);
        ParticleSystem.EmissionModule module = _respawnParticles.emission;

        for (float i = 0; i < spawnTimer; i+=Time.deltaTime)
        {
            //_respawnAudio.pitch = (i / spawnTimer) + 0.5f;
            module.rateOverTime = (i / spawnTimer) * 30;

            yield return 0;
        }
        
        StopRespawnEffects();
        Spawn();
        
        //ObjectiveSpawnManager.instance.SpawnNext();

        yield return 0;
    }

    public void StopRespawnEffects()
    {
        _respawnParticles.gameObject.SetActive(false);
    }
}
