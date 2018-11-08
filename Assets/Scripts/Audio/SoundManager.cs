using System.Collections;
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

}
