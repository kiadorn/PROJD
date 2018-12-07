using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectiveSpawner : NetworkBehaviour {

    public int delayBetweenSpawns;
    public bool StartWithBall;
    public int pointValueTeamWhite;
    public int pointValueTeamBlack;
    public bool independantSpawn;
    private GameObject _ball;
    private ParticleSystem _respawnParticles;
    private int _tiebreakerDelay = 5;

    void Start() {
        _ball = transform.Find("Ball").gameObject;
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
        RoundManager.instance.AddPoint(teamID, amountOfPoints);

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
    }

    public void Spawn() {
        _ball.SetActive(true); //Spawn effekter
        if (_ball.transform.childCount == 0)
            SoundManager.instance.PlayOrbSound(_ball);
    }

    public void Despawn()
    {
        _ball.SetActive(false); //Despawn effekter
    }

    public IEnumerator SpawnTimer(float spawnTimer) {


        yield return new WaitForSeconds(RoundManager.instance.IsTiebreaker ? _tiebreakerDelay : delayBetweenSpawns);
        SoundManager.instance.PlayOrbRespawn(gameObject);
        _respawnParticles.gameObject.SetActive(false);
        _respawnParticles.gameObject.SetActive(true);
        ParticleSystem.EmissionModule module = _respawnParticles.emission;

        for (float i = 0; i < spawnTimer; i+=Time.deltaTime)
        {
            //_respawnAudio.pitch = (i / spawnTimer) + 0.5f;
            //module.rateOverTime = (i / spawnTimer) * 30;

            yield return 0;
        }
        
        Spawn();
        yield return 0;
    }

    public void StopRespawnEffects()
    {
        _respawnParticles.gameObject.SetActive(false);
    }
}
