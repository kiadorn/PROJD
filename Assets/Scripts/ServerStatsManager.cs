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

    [SyncVar] //ineffektivt
    private float _currentRoundTime;

    private float dashCountdown;
    private float dashMAX;

    private float shootCooldown;
    private float shootMAX = 1f;

    [Header("Network")]
    [SyncVar]
    public float maxRotationUpdateLimit = 50f;


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
        if (team1Rounds > RoundsToWin) {
            //PLAYER 1 WINS
            return true;
        }

        else if (team2Rounds > RoundsToWin) {
            //PLAYER 2 WINS
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
        if (isServer)
            RpcSetPlayerMoving(false);
        yield return new WaitForSeconds(waitTimeBeforeStartingRound);
        if (isServer)
            RpcStartRound();
        yield return 0;
    }

    private IEnumerator WaitForEndRound()
    {
        RpcSetPlayerShooting(false);
        Debug.Log("Waiting before next round");
        yield return new WaitForSeconds(waitTimeBeforeEndingRound);
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
