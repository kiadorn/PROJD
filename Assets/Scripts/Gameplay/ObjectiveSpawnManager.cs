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

    /*public int ChooseRandomSpawnIndex()
    {
        return Random.Range(1, spawners.Length);
    }

    public void Spawn(int spawnIndex)
    {

    }


    public void DespawnAll()
    {
        foreach (ObjectiveSpawner spawner in spawners)
        {
            spawner.Despawn(); 
        }
    }

    public void StartRound()
    {
        DespawnAll();

        if (isServer)
            RpcStartSpawnTimer(ChooseRandomSpawnIndex());
    }

    [ClientRpc]
    public void RpcStartSpawnTimer(int spawnIndex)
    {
        ObjectiveSpawner tempSpawner = spawners[0];
        spawners[0] = spawners[spawnIndex];
        spawners[spawnIndex] = tempSpawner;

        spawners[0].Spawn();
    }*/
}
