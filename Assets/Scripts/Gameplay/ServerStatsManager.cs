using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class ServerStatsManager : NetworkBehaviour
{

    public int RoundLength;
    public int RoundsToWin;
    [Header("UI")]
    public Text roundText;
    public Text team1PointsText;
    public Text team2PointsText;
    public Text team1RoundsText;
    public Text team2RoundsText;
    public GameObject team1RoundObjects;
    public GameObject team2RoundObjects;
    public Image shootBar;
    public Image chargeBar;
    public Image dashBar;
    public Text DEAD;
    public Image hitmarker;

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
    public Text startRoundTimerText;
    public GameObject endScreen;
    public Text teamWinnerText;

    private float _serverRoundTimer;
    [SyncVar]
    private int _currentRoundTimer;

    private float dashCountdown;
    private float dashMAX;
    private float dashYellowTime = 0f;
    private float dashGreenTime = 0f;
    public GameObject DashCDTextTimer;

    private float shootCooldown;
    private float shootMAX = 1f;
    private float shootYellowTime = 0f;
    private float shootGreenTime = 0f;
    public GameObject ShootCDTextTimer;

    private int roundStartTimer;

    private float pointAnimationModidier = 0.1f;

    private float teamPointsTextStartSize;
    private float clockStartSize;
    private Vector3 roundStartSize;

    [Header("Network")]
    [SyncVar]
    public float maxRotationUpdateLimit = 50f;

    public GameObject gates;

    [HideInInspector]
    public bool gameStarted = false;

    private void Awake()
    {
        teamPointsTextStartSize = team1PointsText.transform.localScale.x;
        clockStartSize = roundText.transform.localScale.x;
        roundStartSize = team1RoundObjects.transform.GetChild(0).localScale;
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

    void Update()
    {

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

    public void TextAnimation(int teamID)
    {



        /*if (rootAngle < 0 && lerpValue > rootAngle)
            lerpValue = lerpValue - 5;
        else if (rootAngle > 0 && lerpValue < rootAngle)
            lerpValue = lerpValue + 5;
        */
        if (teamID == 1)
        {
            team1PointsText.transform.localScale = new Vector3(teamPointsTextStartSize * 2, teamPointsTextStartSize * 2);
            StartCoroutine(PointAnimation1(team1PointsText));
        }
        else if (teamID == 2)
        {
            team2PointsText.transform.localScale = new Vector3(teamPointsTextStartSize * 2, teamPointsTextStartSize * 2);
            StartCoroutine(PointAnimation2(team2PointsText));
        }

    }

    private IEnumerator PointAnimation1(Text pointText)
    {
        while (pointText.transform.localScale.x >= teamPointsTextStartSize)
        {
            float newValue = pointText.transform.localScale.x - (Time.deltaTime * pointAnimationModidier);
            pointText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }

    private IEnumerator PointAnimation2(Text pointText)
    {
        while (pointText.transform.localScale.x >= teamPointsTextStartSize)
        {
            float newValue = pointText.transform.localScale.x - (Time.deltaTime * pointAnimationModidier);
            pointText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }

    public void AddPoint(int teamID, int amountOfPoints)
    {
        if (!roundIsActive)
            return;

        if (teamID == 1)
        {

            team1Points += amountOfPoints;

        }
        else if (teamID == 2)
        {
            team2Points += amountOfPoints;
        }
        TextAnimation(teamID);

    }

    public void RemovePointsOnPlayer(int teamID)
    {
        if (teamID == 1)
        {
            if (team1Points > team2Points)
                team1Points = (int)((float)team1Points / 2f + (float)team2Points / 2f);
        }
        else if (teamID == 2)
        {
            if (team2Points > team1Points)
                team2Points = (int)((float)team1Points / 2f + (float)team2Points / 2f);
        }
    }

    private void CheckWhoWonRound()
    {
        int winner = 0;
        if (team1Points > team2Points)
        {
            team1Rounds++;
            winner = 1;
        }

        else if (team1Points < team2Points)
        {
            team2Rounds++;
            winner = 2;
        }
        else
        {
            winner = 3;
        }

        if (IsGameOver())
        {
            string winnerText = (team1Rounds > team2Rounds) ? "Team White Won!" : "Team Black Won!";
            RpcShowEndGameScreen(winnerText);
        }
        else
        {
            StartCoroutine(WaitForEndRound());
            RpcShowWinner(winner);
            RpcPlayEndRoundSound(winner);
        }


    }

    [ClientRpc]
    private void RpcShowEndGameScreen(string winnerText)
    {
        endScreen.SetActive(true);
        teamWinnerText.text = winnerText;
    }

    [ClientRpc]
    private void RpcShowWinner(int winner)
    {
        if (winner == 3)
        {
            RoundWinnerTexts[2].enabled = true;
            return;
        }
        StartCoroutine(PopRoundWin(winner));
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playerList)
        {


            if (!player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                continue;
            }

            if ((int)player.GetComponent<PlayerController>().myTeam == winner)
            {
                RoundWinnerTexts[0].enabled = true;
            }
            else
            {
                RoundWinnerTexts[1].enabled = true;
            }
        }
    }

    private IEnumerator PopRoundWin(int winningTeam) {

        bool animatingRound = true;
        bool gettingBigger = true;
        Vector3 roundEndSize = new Vector3((roundStartSize.x * 1.5f), (roundStartSize.y * 1.5f), (roundStartSize.z * 1.5f));

        if (winningTeam == 1) {
            while (animatingRound) {
                if (gettingBigger) {
                    for (int i = 0; i < team1Rounds; i++) {
                        team1RoundObjects.transform.GetChild(i).localScale = Vector3.Lerp(team1RoundObjects.transform.GetChild(i).localScale, roundEndSize, Time.deltaTime * 6);
                    }
                    if (team1RoundObjects.transform.GetChild(0).localScale.x >= roundEndSize.x - 0.01) {
                        gettingBigger = false;
                    }
                    yield return 0;
                }
                else {
                    for (int i = 0; i < team1Rounds; i++) {
                        team1RoundObjects.transform.GetChild(i).localScale = Vector3.Lerp(team1RoundObjects.transform.GetChild(i).localScale, roundStartSize, Time.deltaTime * 4);
                    }
                    if (team1RoundObjects.transform.GetChild(0).localScale.x <= roundStartSize.x + 0.01) {
                        team1RoundObjects.transform.GetChild(0).localScale = roundStartSize;
                        animatingRound = false;
                    }
                    yield return 0;
                }   

            }
        }
        else {
            while (animatingRound) {
                if (gettingBigger) {
                    for (int i = 0; i < team1Rounds; i++) {
                        team2RoundObjects.transform.GetChild(i).localScale = Vector3.Lerp(team2RoundObjects.transform.GetChild(i).localScale, roundEndSize, Time.deltaTime * 4);
                    }
                    if (team2RoundObjects.transform.GetChild(0).localScale.x >= roundEndSize.x - 0.01) {
                        gettingBigger = false;
                    }
                    yield return 0;
                }
                else {
                    for (int i = 0; i < team1Rounds; i++) {
                        team2RoundObjects.transform.GetChild(i).localScale = Vector3.Lerp(team2RoundObjects.transform.GetChild(i).localScale, roundStartSize, Time.deltaTime * 4);
                    }
                    if (team2RoundObjects.transform.GetChild(0).localScale.x <= roundStartSize.x + 0.01) {
                        team2RoundObjects.transform.GetChild(0).localScale = roundStartSize;
                        animatingRound = false;
                    }
                    yield return 0;
                }

            }
        }

    yield return 0;
    }

    [ClientRpc]
    private void RpcHideWinner()
    {
        foreach (Text t in RoundWinnerTexts)
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

    private bool IsGameOver()
    {
        if (team1Rounds >= RoundsToWin)
        {
            SoundManager.instance.PlayLightWin();
            return true;
        }

        else if (team2Rounds >= RoundsToWin)
        {
            SoundManager.instance.PlayDarkWin();
            return true;
        }
        return false;
    }

    public int GetPlayerID()
    {
        _playerID++;
        return _playerID;
    }


    [Command]
    public void CmdStartGame()
    {
        RpcStartGame();
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        Debug.Log("Started Game");
        PrepareRound();
        gameStarted = true;
    }

    public void PrepareRound()
    {
        ObjectiveSpawnManager.instance.DespawnAll();
        _currentRoundTimer = RoundLength;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            SpawnManager.instance.Spawn(player);
        }
        gates.SetActive(true);
        _serverRoundTimer = RoundLength;
        team1Points = 0;
        team2Points = 0;

        StartCoroutine(WaitForStartRound());
    }

    public int GetCurrentRoundTimer()
    {
        return _currentRoundTimer;
    }

    private IEnumerator WaitForStartRound()
    {

        team1Points = 0;
        team2Points = 0;

        if (isServer)
            RpcSetPlayerMoving(true);
        StartCoroutine(StartWaitForRoundTimer());
        yield return new WaitForSeconds(waitTimeBeforeStartingRound - 3);
        SoundManager.instance.StartCountdown();
        team1Points = 0;
        team2Points = 0;
        yield return new WaitForSeconds(3);

        ObjectiveSpawnManager.instance.SpawnNext();

        if (isServer)
            RpcStartRound();

        gates.SetActive(false);
        yield return 0;
    }

    private IEnumerator StartWaitForRoundTimer()
    {
        startRoundTimerText.enabled = true;
        for (int i = waitTimeBeforeStartingRound; i > 0; i--)
        {
            roundStartTimer = i;
            yield return new WaitForSeconds(1f);
        }
        startRoundTimerText.enabled = false;
        yield return 0;
    }

    private IEnumerator WaitForEndRound()
    {
        RpcSetTimeScale(0.5f);
        RpcSetPlayerShooting(false);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<PlayerController>().StopEffects();
        }
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
    public void RpcPlayEndRoundSound(int winner)
    {
        if (winner == 3)
        {
            //TO-DO PLAY TIE
            return;
        }

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                if (player.GetComponent<PlayerController>().myTeamID == winner)
                {
                    SoundManager.instance.PlayRoundWin();
                }
                else
                {
                    SoundManager.instance.PlayRoundLose();
                }
            }
        }

    }

    [ClientRpc]
    public void RpcSetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    //GÖR OM TILL SERVER... eller?
    private void UpdateUI()
    {
        if (_currentRoundTimer <= 10)
        {
            roundText.color = Color.red;
            roundText.transform.localScale = new Vector3(clockStartSize * 1.5f, clockStartSize * 1.5f);
        }
        else
        {
            roundText.color = Color.white;
            roundText.transform.localScale = new Vector3(clockStartSize, clockStartSize);
        }
        roundText.text = _currentRoundTimer.ToString();
        team1PointsText.text = team1Points.ToString();
        team2PointsText.text = team2Points.ToString();
        UpdateRoundsWin(team1Rounds, team1RoundObjects.transform);
        UpdateRoundsWin(team2Rounds, team1RoundObjects.transform);
        UpdateDashBar();
        UpdateDecoyBar();//BORE GÖRAS SÅSMÅNINGOM
        UpdateShootCD();
        startRoundTimerText.text = roundStartTimer.ToString();
    }

    private void UpdateRoundsWin(int roundsWon, Transform parent)
    {
        for (int i = 0; i < roundsWon; i++)
        {
            parent.GetChild(i).GetComponent<Image>().enabled = true;
        }
    }

    public void StartDashTimer(float dashTimer)
    {
        dashMAX = dashTimer;
        dashCountdown = 0;
        dashBar.fillAmount = 0;
        DashCDTextTimer.SetActive(true);
    }

    public void StartDecoyTimer(float dashTimer)//TO DO LATER
    {
        /*
        dashMAX = dashTimer;
        dashCountdown = 0;
        dashBar.fillAmount = 0;
        DashCDTextTimer.SetActive(true);
        */
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
            //dashBar.color = Color.red;
            DashCDTextTimer.GetComponentInChildren<Text>().text = ((int)(dashMAX - dashCountdown + 1)).ToString();
        }
        if (dashBar.fillAmount == 1)
        {
            dashBar.color = Color.green;
            dashYellowTime = 0;
            dashGreenTime = 0;
            DashCDTextTimer.SetActive(false);
        }

        else if (dashBar.fillAmount <= 0.5)
        {
            dashBar.color = Color.Lerp(Color.red, Color.yellow, dashYellowTime);
            dashYellowTime += Time.deltaTime / (dashMAX / 2);
        }
        else if (dashBar.fillAmount > 0.5)
        {
            dashBar.color = Color.Lerp(Color.yellow, Color.green, dashGreenTime);
            dashGreenTime += Time.deltaTime / (dashMAX / 2);
        }


    }

    private void UpdateDecoyBar() //TO DO LATER
    {
        /*if (dashBar.fillAmount < 1)
        {
            dashBar.fillAmount = dashCountdown / dashMAX;
            dashCountdown += Time.deltaTime;
            //dashBar.color = Color.red;
            DashCDTextTimer.GetComponentInChildren<Text>().text = ((int)(dashMAX - dashCountdown + 1)).ToString();
        }
        if (dashBar.fillAmount == 1)
        {
            dashBar.color = Color.green;
            dashYellowTime = 0;
            dashGreenTime = 0;
            DashCDTextTimer.SetActive(false);
        }

        else if (dashBar.fillAmount <= 0.5)
        {
            dashBar.color = Color.Lerp(Color.red, Color.yellow, dashYellowTime);
            dashYellowTime += Time.deltaTime / (dashMAX / 2);
        }
        else if (dashBar.fillAmount > 0.5)
        {
            dashBar.color = Color.Lerp(Color.yellow, Color.green, dashGreenTime);
            dashGreenTime += Time.deltaTime / (dashMAX / 2);
        }*/


    }

    public void UpdateShootCharge(float beamDistance, float beamMax)
    {
        chargeBar.fillAmount = ((beamDistance / (beamMax)));

    }

    public void UpdateShootCD()
    {

        if (shootCooldown > 0)
        {
            if (!ShootCDTextTimer.activeInHierarchy)
            {
                ShootCDTextTimer.SetActive(true);
            }
            ShootCDTextTimer.GetComponentInChildren<Text>().text = ((int)shootCooldown + 1).ToString();
            shootBar.fillAmount = 1 - ((shootCooldown / (shootMAX)));
            shootCooldown -= Time.deltaTime;
            //shootBar.color = new Color32(255, 255, 0, 50);

            chargeBar.fillAmount = 0;



            if (shootBar.fillAmount <= 0.5)
            {
                shootBar.color = Color.Lerp(Color.red, Color.yellow, shootYellowTime);
                shootYellowTime += Time.deltaTime / (shootMAX / 2);
            }
            else if (shootBar.fillAmount > 0.5)
            {
                shootBar.color = Color.Lerp(Color.yellow, Color.green, shootGreenTime);
                shootGreenTime += Time.deltaTime / (shootMAX / 2);
            }
        }
        else
        {
            shootYellowTime = 0;
            shootGreenTime = 0;
            shootBar.color = Color.green;
            ShootCDTextTimer.SetActive(false);
        }
    }

    public IEnumerator ShowHitMarker()
    {
        Color c = hitmarker.color;
        float a = 1;

        while (a > 0)
        {
            hitmarker.color = new Color(c.r, c.g, c.b, a);
            a -= Time.deltaTime * 0.5f;
            yield return 0;
        }
    }
}
