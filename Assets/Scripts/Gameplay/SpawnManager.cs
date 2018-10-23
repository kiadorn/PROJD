﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class SpawnManager : NetworkBehaviour {

    public static SpawnManager instance;
    public Transform[] teamWhiteSpawns;
    public Transform[] teamBlackSpawns;
    public Vector3 spawnOffset;

    private void Awake()
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

    public void Spawn(GameObject player)
    {
        Transform spawn;
        Transform[] listToUse;
        if (player.GetComponent<PlayerController>().myTeamID == 1)
        {
            listToUse = teamWhiteSpawns;
        } else
        {
            listToUse = teamBlackSpawns;
        }
        spawn = listToUse[Random.Range(0, listToUse.Length)];
        player.transform.position = spawn.position + spawnOffset;
        if(player.GetComponent<PlayerController>().isLocalPlayer)
            player.GetComponent<PlayerController>().CmdSendSpawnLocation(player.transform.position);
    }



    
}