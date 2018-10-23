using System.Collections;
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
    public Text DEAD;

    public List<GameObject> playerList;

    public static ServerStatsManager instance;

    private static int _playerID = 0;
    [Header("UI")]
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
    public int waitTimeBeforeStartingRound;
    public int waitTimeBeforeEndingRound;
    public int deathTimer;
    public Image crosshair;
    public Image dashEmpty;
    public Image dashFill;
    public Image shootEmpty;
    public Image shootFill;
    public Text[] RoundWinnerTexts;

    private float _serverRoundTimer;
    [SyncVar]
    private int _currentRoundTimer;

    private float dashCountdown;
    private float dashMAX;

    private float shootCooldown;
    private float shootMAX = 1f;

    [Header("Network")]
    [SyncVar]
    public float maxRotationUpdateLimit = 50f;

    [Header("pls dont kill me")]
    private GameObject go;

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
    }

    void Update() {

        if (isServer)
        {
            if (roundIsActive)
            {
                _serverRoundTimer -= Time.deltaTime;
                _currentRoundTimer = (int)_serverRoundTimer;
                if (_serverRoundTimer <= 0)
                {
                    CheckWhoWonRound();
                    roundIsActive = false;
                    _serverRoundTimer = 0;
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
            RpcShowWinner(1);
        }

        else if (team1Points < team2Points) {
            team2Rounds++;
            RpcShowWinner(2);
        }
        else {
            Debug.Log("ITS A TIE!!!");
            RpcShowWinner(3);
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
    private void RpcShowWinner(int winner)
    {
        if (winner == 3)
        {
            RoundWinnerTexts[2].enabled = true;
            return;
        }

        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playerList)
        {
            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer)
                break;

            if ((int)player.GetComponent<PlayerController>().myTeam == winner)
            {
                RoundWinnerTexts[0].enabled = true;
            } else
            {
                RoundWinnerTexts[1].enabled = true;
            }
        }
    }

    [ClientRpc]
    private void RpcHideWinner()
    {
        foreach(Text t in RoundWinnerTexts)
        {
            t.enabled = false;
        }
    }

    [ClientRpc]
    private void RpcSetPlayerMoving(bool set)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().canMove = set;
            player.GetComponent<PlayerController>().canDash = set;
        }
    }

    [ClientRpc]
    private void RpcSetPlayerShooting(bool set)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().canShoot = set;
        }
    }

    private bool IsGameOver() {
        if (team1Rounds >= RoundsToWin) {
            //PLAYER 1 WINS
            SoundManager.instance.PlayAllyWin();
            return true;
        }

        else if (team2Rounds >= RoundsToWin) {
            //PLAYER 2 WINS
            SoundManager.instance.PlayEnemyWin();
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
            SpawnManager.instance.Spawn(player);
        }
        go.SetActive(true);
        _serverRoundTimer = RoundLength;
        team1Points = 0;
        team2Points = 0;

        StartCoroutine(WaitForStartRound());
    }


    public int GetCurrentRoundTimer() {
        return _currentRoundTimer;
    }


    private void ResetRound() {
        roundIsActive = false;
        StartCoroutine(WaitForStartRound());
    }

    private IEnumerator WaitForStartRound() {
        Debug.Log("Waiting for next round");

        if (isServer)
            RpcSetPlayerMoving(true);

        yield return new WaitForSeconds(waitTimeBeforeStartingRound - 3);
        SoundManager.instance.StartCountdown();
        yield return new WaitForSeconds(3);

        if (isServer)
            RpcStartRound();

        go.SetActive(false);
        yield return 0;
    }

    private IEnumerator WaitForEndRound()
    {
        RpcSetTimeScale(0.5f);
        RpcPlayEndRoundSound();
        RpcSetPlayerShooting(false);
        //RpcSetPlayerMoving(false);
        Debug.Log("Waiting before next round");
        yield return new WaitForSeconds(waitTimeBeforeEndingRound * 0.5f);
        RpcSetTimeScale(1f);
        RpcHideWinner();
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

    [ClientRpc]
    public void RpcPlayEndRoundSound()
    {
        SoundManager.instance.PlayAllyWin(); //TO-DO: Logik för att spela antingen Ally Win och Enemy Win
    }

    [ClientRpc]
    public void RpcSetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    //GÖR OM TILL SERVER... eller?
    private void UpdateUI() {
        roundText.text = _currentRoundTimer.ToString();
        team1PointsText.text = team1Points.ToString();
        team2PointsText.text = team2Points.ToString();
        UpdateRoundsWin(team1Rounds, team1RoundsText.transform);
        UpdateRoundsWin(team2Rounds, team2RoundsText.transform);
        UpdateDashBar();
        UpdateShootCD();
    }

    private void UpdateRoundsWin(int roundsWon, Transform parent)
    {
        for (int i = 0; i < roundsWon; i++)
        {
            parent.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void StartDashTimer(float dashTimer)
    {
        dashMAX = dashTimer;
        dashCountdown = 0;
        dashBar.fillAmount = 0;
    }
    
    public void StartShootTimer(float shootTimer)
    {
        shootMAX = shootTimer;
        shootCooldown = shootMAX;
    }

    private void UpdateDashBar()
    {
        if (dashBar.fillAmount < 1)
        {
            dashBar.fillAmount = dashCountdown / dashMAX;
            dashCountdown += Time.deltaTime;
        }
    }

    public void UpdateShootCharge(float beamDistance, float beamMax)
    {
        shootBar.fillAmount = ((beamDistance / (beamMax * 2.5f)));
        shootBar.color = new Color32(255, 0, 0, 50);
    }

    public void UpdateShootCD()
    {
        if (shootCooldown > 0)
        {
            shootBar.fillAmount = ((shootCooldown / (shootMAX * 2.5f)) - 0.01f);
            shootCooldown -= Time.deltaTime;
            shootBar.color = new Color32(255, 255, 0, 50);
        }
    }
}
