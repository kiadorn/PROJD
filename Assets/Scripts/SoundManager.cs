using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class SoundManager : NetworkBehaviour {
    private GameObject goForPlayerAudio;
    private GameObject goForGateAudio;
    private GameObject goForOrbAudio;

    public static SoundManager instance;

    [Header("Global")]
    public EditedClip backgroundMusic;
    public EditedClip countdownSound;
    public EditedClip allyMatchWinner;
    public EditedClip enemyMatchWinner;
    [Header("Movement")]
    public EditedClip runningSound;
    public EditedClip jumpSound;
    public EditedClip jumpLanding;
    public EditedClip dashSound;
    [Header("Combat")]
    public EditedClip deathSound;
    public EditedClip respawnSound;
    public EditedClip chargeLaser;
    public EditedClip fireLaser;
    public EditedClip hitLaser;
    [Header("Environment")]
    public EditedClip gateAudio;
    public EditedClip orbAudio;
    [Header("Points")]
    public EditedClip allyPoint;
    public EditedClip enemyPoint;

    private void Awake()
    {
        goForPlayerAudio = GameObject.Find("goForPlayerAudio");
        goForGateAudio = GameObject.Find("goForGateAudio");
        goForGateAudio = GameObject.Find("goForOrbAudio");
    }
    private void Start()
    {
        AudioManager.Play2DClip(backgroundMusic);

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

    private void OnEnable()
    {
        foreach (GameObject playerObject in ServerStatsManager.instance.playerList)
        {
            RigidbodyFirstPersonController player = playerObject.GetComponent<RigidbodyFirstPersonController>();

            if (isLocalPlayer)
            {
                player.OnStartJump += PlayJumpSound;
                player.OnStartJump += CmdPlayJumpSound;

            }
        }
    }

    public void StartCountdown()
    {
        AudioManager.Play2DClip(countdownSound);
    }

    public void PlayAllyWin()
    {
        AudioManager.Play2DClip(allyMatchWinner);
    }

    public void PlayEnemyWin()
    {
        AudioManager.Play2DClip(enemyMatchWinner);
    }

    public void PlayRunSound()
    {
        AudioManager.Play3DClip(runningSound, goForPlayerAudio);
    }

    public void PlayJumpSound()
    {
        Debug.Log("Played Sound");
        AudioManager.Play2DClip(jumpSound);
    }

    [Command]
    public void CmdPlayJumpSound()
    {
        RpcPlayJumpSound();
    }

    [ClientRpc]
    public void RpcPlayJumpSound()
    {
        Debug.Log("If before, then server");
        if (!isLocalPlayer)
            PlayJumpSound();
    }


    public void PlayLandingSound()
    {
        AudioManager.Play3DClip(jumpLanding, goForPlayerAudio);
    }

    public void PlayDashSound()
    {
        AudioManager.Play3DClip(dashSound, goForPlayerAudio);
    }

    public void PlayDeathSound()
    {
        AudioManager.Play3DClip(deathSound, goForPlayerAudio);
    }

    public void PlayRespawnSound()
    {
        AudioManager.Play3DClip(respawnSound, goForPlayerAudio);
    }

    public void PlayLaserCharge()
    {
        AudioManager.Play3DClip(chargeLaser, goForPlayerAudio);
    }

    public void PlayFireLaser()
    {
        AudioManager.Play3DClip(fireLaser, goForPlayerAudio);
    }

    public void PlayLaserHit()
    {
        AudioManager.Play3DClip(hitLaser, goForPlayerAudio);
    }

    public void PlayGateSound()
    {
        AudioManager.Play3DClip(gateAudio, goForPlayerAudio);
    }

    public void PlayOrbSound()
    {
        AudioManager.Play3DClip(orbAudio, goForOrbAudio);
    }

    public void PlayAllyPoint()
    {
        AudioManager.Play3DClip(allyPoint, goForPlayerAudio);
    }

    public void PlayEnemyPoint()
    {
        AudioManager.Play3DClip(enemyPoint, goForPlayerAudio);
    }

}
