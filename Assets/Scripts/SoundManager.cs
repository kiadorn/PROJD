using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    private GameObject goForAudio;

    [Header("Global")]
    public EditedClip backgroundMusic;
    public EditedClip countdownSound;
    public EditedClip allyMatchWinner;
    public EditedClip enemyMatchWinner;
    [Header("Movement")]
    public EditedClip runningSound;
    public EditedClip jumpSound;
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
        goForAudio = GameObject.Find("goForAudio");
    }
    private void Start()
    {
        AudioManager.Play2DClip(backgroundMusic);
    }

    public void StartCountdown()
    {
        AudioManager.Play2DClip(countdownSound);
    }

    public void PlayAllyPoint()
    {
        AudioManager.Play3DClip(allyPoint, goForAudio);
    }

    public void PlayEnemyPoint()
    {
        AudioManager.Play3DClip(enemyPoint, goForAudio);
    }

    public void PlayAllyWin()
    {
        AudioManager.Play2DClip(allyMatchWinner);
    }

    public void PlayEnemyWin()
    {
        AudioManager.Play2DClip(enemyMatchWinner);
    }
}
