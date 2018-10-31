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
    private bool _spawning;
    private ParticleSystem _respawnParticles;

    private AudioSource _respawnAudio;
    private GameObject _pointAudio;

    public void SetObjectiveOnCooldown() {
        //_spawning = true;
    }

    void Start() {
        _ball = transform.Find("Ball").gameObject;
        _spawning = !StartWithBall;
        _respawnAudio = transform.Find("goForRespawnAudio").GetComponent<AudioSource>();
        _pointAudio = transform.Find("goForPointAudio").gameObject;
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

        SoundManager.instance.PlayAllyPoint(_pointAudio.gameObject);
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
        _respawnAudio.Play();
    }

    public void Spawn() {
        _ball.SetActive(true); //Spawn effekter
    }

    public void Despawn()
    {
        _ball.SetActive(false); //Despawn effekter
    }

    public IEnumerator SpawnTimer(float spawnTimer) {

        //Play sound!
        //Cool effect
        //yield return new WaitForSeconds(spawnTimer);
        //Remove Cool effect

        _respawnParticles.gameObject.SetActive(true);
        ParticleSystem.EmissionModule module = _respawnParticles.emission;

        for (float i = 0; i < spawnTimer; i+=Time.deltaTime)
        {
            _respawnAudio.pitch = (i / spawnTimer) + 0.5f;
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
        _respawnAudio.Stop();
    }
}
