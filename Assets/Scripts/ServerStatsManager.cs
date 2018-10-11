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

    public List<GameObject> playerList;

    public static ServerStatsManager instance;

    private static int _playerID = 0;
    private int player1Rounds;
    private int player2Rounds;
    private int player1Points;
    private int player2Points;
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

    public void Addpoint(int ID) {
        if(ID == 1) {
            player1Points++;
        }
        else if (ID == 2) {
            player2Points++;
        }
    }

    private void CheckWhoWonRound() {
        if(player1Points > player2Points) {
            player1Rounds++;
        }

        else if (player1Points < player2Points) {
            player2Rounds++;
        }
        else {
            //ITS A TIE!!!
        }

        CheckIfGameOver();
    }

    private void CheckIfGameOver() {
        if (player1Rounds > RoundsToWin) {
            //PLAYER 1 WINS
        }

        else if (player2Rounds > RoundsToWin) {
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
        player1Points = 0;
        player2Points = 0;
        MovePlayersBack();
        yield return new WaitForSeconds(RoundLength);
        //StartCountdown
        StartNewRound();
        yield return 0;
    }

    private void UpdateUI() {
        roundText.text = ((int)_currentRoundTime).ToString();
    }

    private void GameOver() {

    }

    private void MovePlayersBack() {

    }

    private void StartNewRound() {

    }
}
