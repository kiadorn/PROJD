using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoundManager : NetworkBehaviour {

    public int roundLength;
    public int roundsToWin;
    public int waitTimeBeforeStartingRound;
    public int waitTimeBeforeEndingRound;
    public int deathTimer;
    public int roundStartTimer;
    public GameObject gates;

    [SyncVar]
    public int currentRoundTimer;
    private float _serverRoundTimer;

    [SyncVar]
    public int team1Rounds;
    [SyncVar]
    public int team2Rounds;
    [SyncVar]
    public int team1Points;
    [SyncVar]
    public int team2Points;

    [SyncVar]
    //[ReadOnly]
    public bool gameStarted = false;
    private bool roundIsActive = false;

    private SharedUI sharedUI;
    private PersonalUI personalUI;

    public static RoundManager instance;

    void Awake() {
        if (!instance) {
            instance = this;
        }
        else {
            Destroy(instance);
            instance = this;
        }
    }

    void Start() {
        sharedUI = SharedUI.instance;
        personalUI = PersonalUI.instance;
    }

    void Update() {
        if (isServer) {
            if (roundIsActive) {
                _serverRoundTimer -= Time.deltaTime;
                currentRoundTimer = (int)_serverRoundTimer;
                if (_serverRoundTimer <= 0) {
                    CheckWhoWonRound();
                    roundIsActive = false;
                    _serverRoundTimer = 0;
                }
            }
        }
    }

    public void AddPoint(int teamID, int amountOfPoints) {
        if (!roundIsActive)
            return;

        if (teamID == 1) {

            team1Points += amountOfPoints;

        }
        else if (teamID == 2) {
            team2Points += amountOfPoints;
        }
        sharedUI.PointAnimation(teamID);
        TABScoreManager.instance.IncreaseScore(teamID, amountOfPoints);

    }

    public void RemovePointsOnPlayer(int teamID) {
        if (teamID == 1) {
            if (team1Points > team2Points)
                team1Points = (int)((float)team1Points / 2f + (float)team2Points / 2f);
        }
        else if (teamID == 2) {
            if (team2Points > team1Points)
                team2Points = (int)((float)team1Points / 2f + (float)team2Points / 2f);
        }
    }

    public void PrepareRound() {
        ObjectiveSpawnManager.instance.DespawnAll();
        currentRoundTimer = roundLength;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            PlayerSpawnManager.instance.Spawn(player);
        }
        for (int i = 0; i < gates.transform.childCount; i++) {
            gates.transform.GetChild(i).GetComponentInChildren<MeshCollider>().enabled = true;
        }
        _serverRoundTimer = roundLength;
        team1Points = 0;
        team2Points = 0;

        StartCoroutine(WaitForStartRound());
    }

    private void CheckWhoWonRound() {
        int winner = 0;
        if (team1Points > team2Points) {
            team1Rounds++;
            winner = 1;
        }

        else if (team1Points < team2Points) {
            team2Rounds++;
            winner = 2;
        }
        else {
            winner = 3;
        }

        if (IsGameOver()) {
            string winnerText = (team1Rounds > team2Rounds) ? "Light Team Won!" : "Shadow Team Won!";
            RpcShowEndGameScreen(winnerText);
        }
        else {
            StartCoroutine(WaitForEndRound());
            RpcShowWinner(winner);
            RpcPlayEndRoundSound(winner);
        }
    }

    private bool IsGameOver() {
        if (team1Rounds >= roundsToWin) {
            RpcPlayLightWin();
            return true;
        }

        else if (team2Rounds >= roundsToWin) {
            RpcPlayShadowWin();
            return true;
        }
        return false;
    }

    private IEnumerator WaitForStartRound() {
        if (isServer)
            RpcSetPlayerMoving(true);
        StartCoroutine(StartWaitForRoundTimer());
        GateAudio.instance.PlayIdle();
        yield return new WaitForSeconds(waitTimeBeforeStartingRound - 3);
        SoundManager.instance.StartCountdown();
        yield return new WaitForSeconds(3);
        ObjectiveSpawnManager.instance.SpawnNext();
        GateAudio.instance.PlayOpen();
        if (isServer)
            RpcStartRound();

        for (int i = 0; i < gates.transform.childCount; i++) {
            gates.transform.GetChild(i).GetComponentInChildren<MeshCollider>().enabled = false;
        }
        yield return 0;
    }

    private IEnumerator StartWaitForRoundTimer() {
        sharedUI.startRoundTimerText.enabled = true;
        for (int i = waitTimeBeforeStartingRound; i > 0; i--) {
            roundStartTimer = i;
            yield return new WaitForSeconds(1f);
        }
        sharedUI.startRoundTimerText.enabled = false;
        yield return 0;
    }

    private IEnumerator WaitForEndRound() {
        RpcSetTimeScale(0.5f);
        RpcSetPlayerShooting(false);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            player.GetComponent<PlayerController>().StopEffects();
        }
        Debug.Log("Waiting before next round");
        yield return new WaitForSeconds(waitTimeBeforeEndingRound * 0.5f);
        RpcSetTimeScale(1f);
        RpcHideWinner();
        RpcEndRound();
        yield return 0;
    }

    #region ServerOperations
    [Command]
    public void CmdStartGame() {
        RpcStartGame();
    }

    [ClientRpc]
    public void RpcStartGame() {
        PrepareRound();
        gameStarted = true;
    }

    [ClientRpc]
    public void RpcStartRound() {
        if (isServer) {
            RpcSetPlayerMoving(true);
            RpcSetPlayerShooting(true);
        }
        roundIsActive = true;
    }

    [ClientRpc]
    public void RpcEndRound() {
        PrepareRound();
    }

    [ClientRpc]
    private void RpcSetPlayerMoving(bool set) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<PlayerController>().canMove = set;
            player.GetComponent<PlayerController>().canDash = set;
        }
    }

    [ClientRpc]
    private void RpcSetPlayerShooting(bool set) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<PlayerController>().canShoot = set;
        }
    }

    [ClientRpc]
    private void RpcShowWinner(int winner) {

        if (winner == 3) {
            sharedUI.ShowRoundWinner(3);
            return;
        }
        StartCoroutine(SharedUI.instance.PopRoundWin(winner));
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playerList) {


            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                continue;
            }

            if ((int)player.GetComponent<PlayerController>().myTeam == winner) {
                sharedUI.ShowRoundWinner(1);
            }
            else {
                sharedUI.ShowRoundWinner(2);
            }
        }
    }

    [ClientRpc]
    private void RpcHideWinner() {
        sharedUI.roundWinnerText.enabled = false;
    }

    [ClientRpc]
    public void RpcSetTimeScale(float scale) {
        Time.timeScale = scale;
    }

    [ClientRpc]
    public void RpcPlayEndRoundSound(int winner) {
        if (winner == 3) {
            //TO-DO PLAY TIE
            return;
        }

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                if (player.GetComponent<PlayerController>().myTeamID == winner) {
                    SoundManager.instance.PlayRoundWin();
                }
                else {
                    SoundManager.instance.PlayRoundLose();
                }
            }
        }
    }

    [ClientRpc]
    private void RpcPlayLightWin() {
        SoundManager.instance.PlayLightWin();
    }

    [ClientRpc]
    private void RpcPlayShadowWin() {
        SoundManager.instance.PlayDarkWin();
    }

    [ClientRpc]
    private void RpcShowEndGameScreen(string winnerText) {
        sharedUI.endGameScreen.SetActive(true);
        sharedUI.teamWinnerText.text = winnerText;
    }

    #endregion



}
