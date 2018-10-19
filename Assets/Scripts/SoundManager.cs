using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    private GameObject goForPlayerAudio;
    private GameObject goForGateAudio;
    private GameObject goForOrbAudio;

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
        AudioManager.Play2DClip(jumpSound);
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
