using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerStatsManager : NetworkBehaviour {

    public int RoundLength;
    public int RoundsToWin;
    public int RoundResetTime;
    public int TimeBeforeResettingPoints;
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
    [SyncVar]
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("SPAAACE");
                StartGame();
            }

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

    public void StartGame() {
        _currentRoundTime = RoundLength;
        roundIsActive = true;
    }

    public int GetCurrentRoundTimer() {
        return (int)_currentRoundTime;
    }


    private void ResetRound() {
        roundIsActive = false;
        StartCoroutine(WaitForNextRound());
    }

    private IEnumerator WaitForNextRound() {
        //Player X WON THE ROUND!
        yield return new WaitForSeconds(TimeBeforeResettingPoints);
        team1Points = 0;
        team2Points = 0;
        MovePlayersBack();
        yield return new WaitForSeconds(RoundLength);
        //StartCountdown
        StartNewRound();
        yield return 0;
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

    private void StartNewRound() {

    }
}
