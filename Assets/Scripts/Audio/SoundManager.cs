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
    public EditedClip laserCooldownFinished;
    public EditedClip chargeLaser;
    public EditedClip fireLaser;
    public EditedClip hitLaser;
    [Header("Environment")]
    public EditedClip gateAudio;
    public EditedClip orbAudio;
    [Header("Points")]
    public EditedClip allyPoint;
    public EditedClip enemyPoint;
    [Header("Colors")]
    public EditedClip OwnNewArea;
    public EditedClip OtherNewArea;

    private void Awake()
    {
        goForGateAudio = GameObject.Find("goForGateAudio");
        goForOrbAudio = GameObject.Find("goForOrbAudio");
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

    public void AddSoundOnStart(PlayerController player)
    {
        player.OnStartJump += PlayJumpSound;
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

    public void PlayAllyWin()
    {
        AudioManager.Play2DClip(allyMatchWinner);
    }

    public void PlayEnemyWin()
    {
        AudioManager.Play2DClip(enemyMatchWinner);
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
        if (!goForPlayerAudio.GetComponent<PlayerController>().isLocalPlayer)
            PlayJumpSound();
    }

    public void PlayLandingSound()
    {
        AudioManager.Play3DClip(jumpLanding, goForPlayerAudio);
    }

    public void PlayDashSound(int playerID)
    {
        AudioManager.Play3DClip(dashSound, FindPlayer(playerID));
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

    public void PlayRespawnSound()
    {
        AudioManager.Play3DClip(respawnSound, goForPlayerAudio);
    }

    public void PlayLaserCharge(int playerID)
    {
        print("innan");
        if(chargeLaser.GetSource() == null)
            AudioManager.Play3DClip(chargeLaser, FindPlayer(playerID));

        print("efter");
    }

    public void StopCharge() {
        chargeLaser.Stop();

        print("efter");
    }
    public void PlayLaserCooldownFinished()
    {
        AudioManager.Play2DClip(laserCooldownFinished);
    }

    public void PlayFireLaser(int playerID)
    {
        AudioManager.Play3DClip(fireLaser, FindPlayer(playerID));
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

    public void PlayAllyPoint(GameObject OrbLocation)
    {
        AudioManager.Play3DClip(allyPoint, OrbLocation);
    }

    public void PlayEnemyPoint()
    {
        AudioManager.Play3DClip(enemyPoint, goForPlayerAudio);
    }

}
