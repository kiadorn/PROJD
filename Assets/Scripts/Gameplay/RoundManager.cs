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
    public float slowMotionScale = 0.2f;
    public int deathTimer;
    public int roundStartTimer;
    public GameObject gates;
    [SerializeField]
    private StringVariable _player1Name;
    [SerializeField]
    private StringVariable _player2Name;

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
    public float team1killstreak = 1f;
    [SyncVar]
    public float team2killstreak = 1f;

    [SyncVar]
    public bool gameStarted = false;
    private bool roundIsActive = false;
    [HideInInspector]
    public bool tutorialActive = false;  

    private SharedUI sharedUI;
    private PersonalUI personalUI;
    [Header("IntroToLevel")]
    [SerializeField] private IntroCameraRotation introCameraRotator;
    [SerializeField] private Image blackScreen;
    [SerializeField] private CanvasGroup canvasElement;
    public float BlackScreenSpeed;
    public float IntroCameraTime = 5f;
    private WaitForSeconds _cameraWait;
    public static RoundManager instance;
    [Header("Tiebreaker")]
    [SerializeField] private int roundLengthTiebreaker;
    public bool IsTiebreaker;
    public bool IsOverTime = false;

    public delegate void RoundEvent();
    public RoundEvent OnStartGame;


    void Awake() {
        if (!instance) {
            instance = this;
        }
        else {
            Destroy(instance);
            instance = this;
        }
        if (blackScreen) {
            blackScreen.gameObject.SetActive(true);
        }
        _cameraWait = new WaitForSeconds(IntroCameraTime);
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
                    _serverRoundTimer = 0;

                    if (IsTiebreaker && (team1Points == team2Points))
                    {
                        IsOverTime = true;
                        return;
                    }
                    CheckWhoWonRound();
                    roundIsActive = false;
                }
            }
        }
    }

    public void AddPoint(int teamID, int amountOfPoints) {
        if (!roundIsActive && !tutorialActive)
            return;

        if (teamID == 1) {
            team1Points += amountOfPoints;
        }
        else if (teamID == 2) {
            team2Points += amountOfPoints;
        }
        sharedUI.PointAnimation(teamID);
        sharedUI.AnimateAddedPoints(teamID, amountOfPoints);
        
        TABScoreManager.instance.IncreaseScore(teamID, amountOfPoints);
    }

    public void AddKillstreak (int teamID)
    {
        if (!roundIsActive && !tutorialActive)
            return;

        if (teamID == 1)
        {
            team1killstreak += 0.1f;
        }

        if (teamID == 2)
        {
            team2killstreak += 0.1f;
        }
        sharedUI.MultiplierAnimation(teamID);
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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            PlayerSpawnManager.instance.Spawn(player);
        }
        for (int i = 0; i < gates.transform.childCount; i++) {
            gates.transform.GetChild(i).GetComponentInChildren<MeshCollider>().enabled = true;
        }
        currentRoundTimer = IsTiebreaker ? roundLengthTiebreaker : roundLength;
        _serverRoundTimer = currentRoundTimer;
        team1Points = 0;
        team2Points = 0;
        team1killstreak = 1f;
        team2killstreak = 1f;
        sharedUI.team1PointsKillText.text = "";
        sharedUI.team2PointsKillText.text = "";
        sharedUI.team1PointsMultiplier.text = "";
        sharedUI.team2PointsMultiplier.text = "";
        StartCoroutine(WaitForStartRound());
    }

    private void CheckWhoWonRound() {
        int winner = 0;
        IsTiebreaker = false;
        if (team1Points > team2Points) {
            team1Rounds++;
            winner = 1;
        }

        else if (team1Points < team2Points) {
            team2Rounds++;
            winner = 2;
        }
        else {
            if (team1Rounds == 2 || team2Rounds == 2)
            {
                IsTiebreaker = true;
                winner = 4;
            }
            else
            {
                team1Rounds++;
                team2Rounds++;
                winner = 3;
            }
            
        }
        Debug.Log(winner);
        if (IsGameOver()) {
            string winnerText = (team1Rounds > team2Rounds) ? _player1Name.Value + "\nwon the game!" : _player2Name.Value + "\nwon the game!";
            int winnerTeam = (team1Rounds > team2Rounds) ? 1 : 2;
            RpcShowEndGameScreen(winnerText, winnerTeam);
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
        StartCoroutine(StartWaitForRoundTimer());
        GateAudio.instance.PlayIdle();
        yield return new WaitForSeconds(waitTimeBeforeStartingRound - 4);
        SoundManager.instance.StartCountdown();
        yield return new WaitForSeconds(4);
        if (!IsTiebreaker) ObjectiveSpawnManager.instance.SpawnNext();
        ObjectiveSpawnManager.instance.SpawnAllIndependant();
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
        RpcSetTimeScale(slowMotionScale);
        RpcAllowPlayerShooting(false);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            player.GetComponent<PlayerController>().StopEffects();
        }

        yield return new WaitForSeconds(waitTimeBeforeEndingRound * slowMotionScale);
        RpcSetTimeScale(1f);
        RpcHideRoundWinnerText();
        RpcEndRound();
        yield return 0;
    }

    private IEnumerator PrepareGame()
    {
        if (OnStartGame != null)
            OnStartGame();

        SetPlayersMoving(false);
        AllowPlayerShooting(false);
        canvasElement.alpha = 0;
        introCameraRotator.ChangeIntroCamera(50, true); //Sets this camera to render and starts it's rotation.
        for (float i = 1; blackScreen.color.a > 0; i -= Time.deltaTime * BlackScreenSpeed)
        { //Fades the Blackscreen out
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, i);
            yield return 0;
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0);
        yield return _cameraWait;                                                                          //Waits to look at the map                    
        for (float i = 0; blackScreen.color.a < 1; i += Time.deltaTime * BlackScreenSpeed)
        {                //Fades it in again;
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, i);
            yield return 0;
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 1);
        introCameraRotator.ChangeIntroCamera(-50, false);  //Stops the camera and set it to low priority;
        introCameraRotator.TurnOffIntroCamera(); //Turns the camera off.
        PrepareRound();
        gameStarted = true;
        yield return new WaitForSeconds(1f);
        SetPlayersMoving(true);
        AllowPlayerShooting(true);
        for (float i = 1; blackScreen.color.a > 0; i -= Time.deltaTime * BlackScreenSpeed)
        { //Fades the Blackscreen out again
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, i);
            canvasElement.alpha = 1 - i;
            yield return 0;
        }
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0);
    }

    private void SetPlayersMoving(bool set) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<PlayerController>().canShoot = set;
        }
    }

    private void AllowPlayerShooting(bool set) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            player.GetComponent<PlayerController>().canShoot = set;
        }
    }

    #region ServerOperations
    [Command]
    public void CmdStartGame() {
        RpcStartGame();
    }

    [ClientRpc]
    public void RpcStartGame() {
        StartCoroutine(PrepareGame());
    }

    [ClientRpc]
    public void RpcStartRound() {
        if (isServer) {
            RpcSetPlayerMoving(true);
            RpcAllowPlayerShooting(true);
        }
        roundIsActive = true;
    }

    [ClientRpc]
    public void RpcEndRound() {
        PrepareRound();
    }

    [ClientRpc]
    private void RpcSetPlayerMoving(bool set) {
        SetPlayersMoving(set);
    }

    [ClientRpc]
    private void RpcAllowPlayerShooting(bool set) {
        AllowPlayerShooting(set);
    }

    [ClientRpc]
    private void RpcShowWinner(int winner) {
        print(winner);

        if (winner == 3) {
            sharedUI.ShowRoundWinner(3);
            return;
        }

        if (winner == 4)
        {
            sharedUI.ShowRoundWinner(4);
            return;
        }
        //StartCoroutine(SharedUI.instance.PopRoundWin(winner));
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
    private void RpcHideRoundWinnerText() {
        sharedUI.roundWinnerText.enabled = false;
    }

    [ClientRpc]
    public void RpcSetTimeScale(float scale) {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;
    }

    [ClientRpc]
    public void RpcPlayEndRoundSound(int winner) {
        if (winner == 3) {
            SoundManager.instance.PlayRoundTie();
            return;
        }

        if (winner == 4)
        {
            SoundManager.instance.PlayRoundTie();
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
    private void RpcShowEndGameScreen(string winnerText, int winningTeam) {
        sharedUI.endGameScreen.SetActive(true);
        sharedUI.teamWinnerText.text = winnerText;
        sharedUI.endImage.GetComponent<Image>().sprite = (winningTeam == 1) ? sharedUI.yellowVictory : sharedUI.purpleVictory;
    }

    #endregion

 

}
