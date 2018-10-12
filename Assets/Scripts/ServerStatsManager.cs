using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ServerStatsManager : NetworkBehaviour {

    public int RoundLength;
    public int RoundsToWin;
    public int RoundResetTime;
    public Text roundText;
    public Text team1PointsText;
    public Text team2PointsText;

    public List<GameObject> playerList;

    public static ServerStatsManager instance;

    private static int _playerID = 0;
    private int team1Rounds;
    private int team2Rounds;
    [SyncVar]
    private int team1Points;
    [SyncVar]
    private int team2Points;
    private int currentRound;
    private bool roundIsActive = false;
    public int WaitTimeBeforeStartingRound;
    [SyncVar] //ineffektivt
    private float _currentRoundTime;


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
            //ITS A TIE!!!
        }

        CheckIfGameOver();
    }

    private void CheckIfGameOver() {
        if (team1Rounds > RoundsToWin) {
            //PLAYER 1 WINS
        }

        else if (team2Rounds > RoundsToWin) {
            //PLAYER 2 WINS
        }
    }

    public int GetPlayerID() {
        _playerID++;
        return _playerID;
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
            player.GetComponent<RigidbodyFirstPersonController>().Death();
        }
        _currentRoundTime = RoundLength;
        team1Points = 0;
        team2Points = 0;

        StartCoroutine(WaitForNextRound());
    }

    public void StartRound()
    {
        
    }

    public int GetCurrentRoundTimer() {
        return (int)_currentRoundTime;
    }


    private void ResetRound() {
        roundIsActive = false;
        StartCoroutine(WaitForNextRound());
    }

    private IEnumerator WaitForNextRound() {
        Debug.Log("Waiting for next round");
        yield return new WaitForSeconds(WaitTimeBeforeStartingRound);
        StartNewRound();
        yield return 0;
    }

    private void StartNewRound()
    {
        Debug.Log("Starting Round!");

    }

    //GÖR OM TILL SERVER
    private void UpdateUI() {
        roundText.text = ((int)_currentRoundTime).ToString();
        team1PointsText.text = team1Points.ToString();
        team2PointsText.text = team2Points.ToString();
    }

    private void GameOver() {

    }

    private void MovePlayersBack() {

    }


}
