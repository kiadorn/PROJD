using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectiveSpawnManager : NetworkBehaviour {

    public static ObjectiveSpawnManager instance;

    public List<ObjectiveSpawner> spawnedSpawners = new List<ObjectiveSpawner>();
    public List<ObjectiveSpawner> unspawnedSpawners = new List<ObjectiveSpawner>();
    public List<ObjectiveSpawner> independantSpawners = new List<ObjectiveSpawner>();
    public int spawnTimer;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }

    public void SpawnNext()
    {
        if (unspawnedSpawners.Count == 0)
            return;

        if (isServer)
            RpcStartSpawnTimer(ChooseRandomSpawnIndex());
    }

    public void SpawnMe(ObjectiveSpawner spawner) {

        if (isServer) {
            RpcStartSpawnIndependantTimer(independantSpawners.IndexOf(spawner));
        }
    }

    public void SpawnAllIndependant()
    {
        foreach (ObjectiveSpawner spawner in independantSpawners)
        {
            SpawnMe(spawner);
        }
    }

    public void Despawn(ObjectiveSpawner spawner)
    {
        spawnedSpawners.Remove(spawner);
        unspawnedSpawners.Add(spawner);
    }

    public void DespawnAll() {
        foreach (ObjectiveSpawner spawner in spawnedSpawners) {
            ResetSpawner(spawner);
        }
        foreach (ObjectiveSpawner spawner in independantSpawners)
        {
            ResetSpawner(spawner);
        }
        unspawnedSpawners.AddRange(spawnedSpawners);
        spawnedSpawners.Clear();

    }

    private void ResetSpawner(ObjectiveSpawner spawner)
    {
        spawner.StopRespawnEffects();
        spawner.StopAllCoroutines();
        spawner.Despawn();
        if (spawner.StartWithBall && !RoundManager.instance.IsTiebreaker)
            spawner.Spawn();
    }

    public int ChooseRandomSpawnIndex() {
        return Random.Range(0, unspawnedSpawners.Count);
    }

    [ClientRpc]
    public void RpcStartSpawnTimer(int spawnIndex)
    {
        unspawnedSpawners[spawnIndex].StartRespawn();
        spawnedSpawners.Add(unspawnedSpawners[spawnIndex]);
        unspawnedSpawners.RemoveAt(spawnIndex);
    }

    [ClientRpc]
    public void RpcStartSpawnIndependantTimer(int spawnIndex)
    {
        independantSpawners[spawnIndex].StartRespawn();
    }
}
