using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectiveSpawnManager : NetworkBehaviour {

    public static ObjectiveSpawnManager instance;

    //public ObjectiveSpawner[] spawners;

    public List<ObjectiveSpawner> spawnedSpawners = new List<ObjectiveSpawner>();
    public List<ObjectiveSpawner> unspawnedSpawners = new List<ObjectiveSpawner>();
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



    public void Spawn(int spawnIndex)
    {

    }




    public void StartRound()
    {
        DespawnAll();

        if (isServer)
            RpcStartSpawnTimer(ChooseRandomSpawnIndex());
    }

    public void DespawnAll() {
        foreach (ObjectiveSpawner spawner in spawnedSpawners) {
            spawner.Despawn();
        }

        unspawnedSpawners.AddRange(spawnedSpawners);
        unspawnedSpawners.Clear();
    }

    public int ChooseRandomSpawnIndex() {
        return Random.Range(0, unspawnedSpawners.Count);
    }

    [ClientRpc]
    public void RpcStartSpawnTimer(int spawnIndex)
    {
        unspawnedSpawners[spawnIndex].Spawn();
        spawnedSpawners.Add(unspawnedSpawners[spawnIndex]);
        unspawnedSpawners.RemoveAt(spawnIndex); //BE CAUTIOUS :OOOOOO
        
    }
}
