﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ServerStatsManager : NetworkBehaviour {

    public int RoundLength;
    public int RoundsToWin;
    [Header("UI")]
    public Text roundText;
    public Text team1PointsText;
    public Text team2PointsText;
    public Text team1RoundsText;
    public Text team2RoundsText;
    public Image shootBar;
    public Image dashBar;

    public List<GameObject> playerList;

    public static ServerStatsManager instance;

    private static int _playerID = 0;
    [SyncVar]
    private int team1Rounds;
    [SyncVar]
    private int team2Rounds;
    [SyncVar]
    private int team1Points;
    [SyncVar]
    private int team2Points;
    private int currentRound;
    private bool roundIsActive = false;
    public int WaitTimeBeforeStartingRound;
    public int WaitTimeBeforeEndingRound;
    [SyncVar] //ineffektivt
    private float _currentRoundTime;

    [Header("Bullshit Ass Math")]
    private float dashCountdown;
    private float dashMAX;

    [Header("pls dont kill me")]
    private GameObject go;
    private GameObject SM;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        } else
        {
            Destroy(instance);
            instance = this;
        }
    }

    //haha
    private void Start()
    {
       go = GameObject.Find("Gates");
       SM = GameObject.Find("SoundManager");
    }

    void Update() {

        if (isServer)
        {
            if (roundIsActive)
            {
                _currentRoundTime -= Time.deltaTime;
                if (_currentRoundTime <= 0)
                {
                    CheckWhoWonRound();
                    roundIsActive = false;
                    _currentRoundTime = 0;
                }
            }
        }

        UpdateUI();
    }

    public void AddPoint(int ID) {
        if(ID == 1) {
            team1Points++;
        }
        else if (ID == 2) {
            team2Points++;
        }
    }

    private void CheckWhoWonRound() {
        if(team1Points > team2Points) {
            team1Rounds++;
        }

        else if (team1Points < team2Points) {
            team2Rounds++;
        }
        else {
            Debug.Log("ITS A TIE!!!");
        }

        if (IsGameOver())
        {
            Debug.Log("End Game");
        } else
        {
            StartCoroutine(WaitForEndRound());
        }
    }

    [ClientRpc]
    private void RpcSetPlayerMoving(bool set)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<RigidbodyFirstPersonController>().canMove = set;
            player.GetComponent<RigidbodyFirstPersonController>().canDash = set;
        }
    }

    [ClientRpc]
    private void RpcSetPlayerShooting(bool set)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<RigidbodyFirstPersonController>().canShoot = set;
        }
    }

    private bool IsGameOver() {
        if (team1Rounds >= RoundsToWin) {
            //PLAYER 1 WINS
            SM.GetComponent<SoundManager>().PlayAllyWin();
            return true;
        }

        else if (team2Rounds >= RoundsToWin) {
            //PLAYER 2 WINS
            SM.GetComponent<SoundManager>().PlayEnemyWin();
            return true;
        }
        return false;
    }

    public int GetPlayerID() {
        _playerID++;
        return _playerID;
    }


    [Command]
    public void CmdStartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    public void RpcStartGame() {
        Debug.Log("Started Game");
        PrepareRound();
        
    }

    public void PrepareRound()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.GetComponent<RigidbodyFirstPersonController>().Death();
        }
        go.SetActive(true);
        _currentRoundTime = RoundLength;
        team1Points = 0;
        team2Points = 0;

        StartCoroutine(WaitForStartRound());
    }


    public int GetCurrentRoundTimer() {
        return (int)_currentRoundTime;
    }


    private void ResetRound() {
        roundIsActive = false;
        StartCoroutine(WaitForStartRound());
    }

    private IEnumerator WaitForStartRound() {
        Debug.Log("Waiting for next round");
        if (isServer) {
            RpcSetPlayerMoving(true);
        }
        yield return new WaitForSeconds(WaitTimeBeforeStartingRound - 3);
        SM.GetComponent<SoundManager>().StartCountdown();
        yield return new WaitForSeconds(3);
        if (isServer)
            RpcStartRound();
        go.SetActive(false);
        yield return 0;
    }

    private IEnumerator WaitForEndRound()
    {
        RpcSetPlayerShooting(false);
        RpcSetPlayerMoving(false);
        Debug.Log("Waiting before next round");
        yield return new WaitForSeconds(WaitTimeBeforeEndingRound);
        RpcEndRound();
        yield return 0;
    }

    [ClientRpc]
    public void RpcStartRound()
    {
        Debug.Log("Starting Round!");
        if (isServer)
        {
            RpcSetPlayerMoving(true);
            RpcSetPlayerShooting(true);
        }
        roundIsActive = true;
    }

    [ClientRpc]
    public void RpcEndRound()
    {
        PrepareRound();
    }

    //GÖR OM TILL SERVER... eller?
    private void UpdateUI() {
        roundText.text = ((int)_currentRoundTime).ToString();
        team1PointsText.text = team1Points.ToString();
        team2PointsText.text = team2Points.ToString();
        team1RoundsText.text = team1Rounds.ToString();
        team2RoundsText.text = team2Rounds.ToString();

        UpdateDashBar();
    }

    public void StartDashTimer(float dashTimer)
    {
        dashMAX = dashTimer;
        dashCountdown = dashTimer;
    }

    private void UpdateDashBar()
    {
        if (dashCountdown > 0)
        {
            dashBar.fillAmount = ((dashCountdown / (dashMAX * 2.5f)) + 0.49f);
            dashCountdown -= Time.deltaTime;
        }
    }

    public void UpdateShootCharge(float beamDistance, float beamMax)
    {
            shootBar.fillAmount = ((beamDistance / (beamMax * 2.5f)));
    }

    private void GameOver() {

    }

    private void MovePlayersBack() {

    }


}
