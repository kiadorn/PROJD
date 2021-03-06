﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class SoundManager : NetworkBehaviour {
    private GameObject goForPlayerAudio;
    //private GameObject goForGateAudio;
    private GameObject goForOrbAudio;

    public static SoundManager instance;

    [Header("Global")]
    public EditedClip backgroundMusic;
    public EditedClip countdownSound;
    public EditedClip allyMatchWinner;
    public EditedClip enemyMatchWinner;
    public EditedClip roundWin;
    public EditedClip roundLose;
    public EditedClip roundTie;
    public EditedClip actionUnavailable;
    [Header("Menu")]
    public EditedClip ButtonHover;
    public EditedClip ButtonSelect;
    public EditedClip ButtonSelect2;
    [Header("Lobby")]
    public EditedClip ReadyUp;
    [Header("Tutorial")]
    public EditedClip TutorialProgressSound;
    public EditedClip TutorialNextRoom;
    [Header("Movement")]
    public EditedClip runningSound;
    public EditedClip jumpSound;
    public EditedClip jumpLanding;
    public EditedClip dashSound;
    [Header("Combat")]
    public EditedClip deathSound;
    public EditedClip respawnSound;
    public EditedClip laserCooldownFinished;
    public EditedClip dashCooldownFinished;
    public EditedClip chargeLaser;
    public EditedClip fireLaser;
    public EditedClip hitLaser;
    [Header("Abilities")]
    public EditedClip decoyUse;
    public EditedClip decoyPoof;
    public EditedClip decoyCooldownFinished;
    [Header("Environment")]
    public EditedClip gateIdleAudio;
    public EditedClip gateOpenAudio;
    public EditedClip orbIdleAudio;
    public EditedClip orbRespawnAudio;
    [Header("Points")]
    public EditedClip allyPoint;
    public EditedClip enemyPoint;
    [Header("Colors")]
    public EditedClip OwnNewArea;
    public EditedClip OtherNewArea;

    private void Awake()
    {
        //goForGateAudio = GameObject.Find("goForGateAudio");
        goForOrbAudio = GameObject.Find("goForOrbAudio");
    }
    private void Start()
    {
        //AudioManager.Play2DClip(backgroundMusic);

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

    public void AddSoundOnStart(PlayerController player)
    {
        //player.OnStartJump += PlayJumpSound;
        //player.EventOnDeath += PlayDeathSound;


    }

    public void SetPlayerOrigin(GameObject player)
    {
        goForPlayerAudio = player;
    }

    public GameObject FindPlayer(int playerID)
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<PlayerID>().playerID == playerID)
            {
                return player;
            }
        }

        return null;
    }

    public void StartCountdown()
    {
        AudioManager.Play2DClip(countdownSound);
    }

    public void PlayLightWin()
    {
        AudioManager.Play2DClip(allyMatchWinner);
    }

    public void PlayDarkWin()
    {
        AudioManager.Play2DClip(enemyMatchWinner);
    }

    public void PlayRoundWin()
    {
        AudioManager.Play2DClip(roundWin);
    }

    public void PlayRoundLose()
    {
        AudioManager.Play2DClip(roundLose);
    }

    public void PlayRoundTie()
    {
        AudioManager.Play2DClip(roundTie);
    }

    public void PlayNewArea(bool invisible) {
        EditedClip clipToPlay;
        if (invisible) {
            clipToPlay = OwnNewArea;
        }
        else {
            clipToPlay = OtherNewArea;
        }
        AudioManager.Play2DClip(clipToPlay);
    }

    public void PlayJumpSound()
    {
        //AudioManager.Play2DClip(jumpSound);
    }

    [Command]
    public void CmdPlayJumpSound()
    {
        RpcPlayJumpSound();
    }

    [ClientRpc]
    public void RpcPlayJumpSound()
    {
        if (!goForPlayerAudio.GetComponent<PlayerController>().isLocalPlayer)
            PlayJumpSound();
    }

    public void PlayLandingSound(float airTime)
    {
        jumpLanding.Volume = airTime;
        jumpLanding.Volume = Mathf.Clamp(jumpLanding.Volume, 0f, 1f);
        AudioManager.Play3DClip(jumpLanding, goForPlayerAudio);
    }

    public void PlayDashSound(int playerID)
    {
        AudioManager.Play3DClip(dashSound, FindPlayer(playerID));
    }

    public void PlayDecoyUse()
    {
        AudioManager.Play2DClip(decoyUse);
    }

    public void PlayDecoyPoof(GameObject decoy)
    {
        AudioManager.Play3DClip(decoyPoof, decoy);
    }

    public void PlayDecoyCooldownFinished()
    {
        AudioManager.Play2DClip(decoyCooldownFinished);
    }

    public void PlayActionUnavailable()
    {
        if (!AudioManager.IsSoundPlaying(actionUnavailable))
            AudioManager.Play2DClip(actionUnavailable);
    }

    //[Command]
    //public void CmdPlayDashSound(int playerID)
    //{
    //    print("Command sent");
    //    if (isServer)
    //    {
    //        print("Approved by server");
    //        RpcPlayDashSound(playerID);

    //    }
    //}

    //[ClientRpc]
    //public void RpcPlayDashSound(int playerID)
    //{
    //    print("Server sent");
    //    if (!(this.FindPlayer(playerID).GetComponent<PlayerController>().isLocalPlayer))
    //    {
    //        print(playerID.ToString() + " is playing sound");
    //        PlayDashSound(playerID);
    //    }
    //}

    public void PlayDeathSound(int playerID)
    {
        AudioManager.Play3DClip(deathSound, FindPlayer(playerID));
    }

    public void PlayRespawnSound(GameObject source)
    {
        respawnSound.Pitch = Random.Range(0.99f, 1.01f);
        AudioManager.Play3DClip(respawnSound, source);
    }

    public void PlayLaserCharge(int playerID)
    {
        if(chargeLaser.GetSource() == null)
            AudioManager.Play3DClip(chargeLaser, FindPlayer(playerID));
    }

    public void StopCharge() {
        chargeLaser.Stop();
    }
    public void PlayLaserCooldownFinished()
    {
        AudioManager.Play2DClip(laserCooldownFinished);
    }

    public void PlayDashCooldownFinished()
    {
        AudioManager.Play2DClip(dashCooldownFinished);
    }
    public void PlayFireLaser(int playerID)
    {
        AudioManager.Play3DClip(fireLaser, FindPlayer(playerID));
    }

    public void PlayLaserHit()
    {
        AudioManager.Play2DClip(hitLaser);
    }

    public void PlayGateSound(GameObject GateLocation)
    {
        AudioManager.Play3DClip(gateIdleAudio, GateLocation);
    }

    public void PlayGateOpen(GameObject GateLocation)
    {
        AudioManager.Play3DClip(gateOpenAudio, GateLocation);
    }

    public void PlayOrbSound(GameObject OrbLocation)
    {
        AudioManager.Play3DClip(orbIdleAudio, OrbLocation);
    }

    public void PlayOrbRespawn(GameObject OrbLocation)
    {
        AudioManager.Play3DClip(orbRespawnAudio, OrbLocation);
    }

    public void PlayAllyPoint(GameObject OrbLocation)
    {
        AudioManager.Play3DClip(allyPoint, OrbLocation);
    }

    public void PlayEnemyPoint(GameObject OrbLocation)
    {
        AudioManager.Play3DClip(enemyPoint, OrbLocation);
    }

    public void PlayButtonHover()
    {
        AudioManager.Play2DClip(ButtonHover);
    }

    public void PlayButtonPress()
    {
        AudioManager.Play2DClip(ButtonSelect);
    }

    public void PlayButtonPress2()
    {
        AudioManager.Play2DClip(ButtonSelect2);
    }

    public void PlayPlayerReady()
    {
        AudioManager.Play2DClip(ReadyUp);
    }

    public void PlayTutorialProgressSound()
    {
        AudioManager.Play2DClip(TutorialProgressSound);
    }

    public void PlayTutorialNextRoom(GameObject Door)
    {
        AudioManager.Play3DClip(TutorialNextRoom, Door);
    }
}
