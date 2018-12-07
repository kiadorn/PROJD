using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SharedUI : MonoBehaviour {

    [Header("Start of Round")]
    public Text startRoundTimerText;
    [Header("During Rounds")]
    public TextMeshProUGUI roundTimerText;
    public TextMeshProUGUI team1PointsText;
    public TextMeshProUGUI team2PointsText;
    public TextMeshProUGUI team1PointsMultiplier;
    public TextMeshProUGUI team2PointsMultiplier;
    public TextMeshProUGUI team1TextToAnimate;
    public TextMeshProUGUI team2TextToAnimate;
    public GameObject team1PopObjects;
    public GameObject team1RoundObjectsBackrounds;
    public GameObject team2PopObjects;
    public GameObject team2RoundObjectsBackrounds;
    [Header("End of Round")]
    public Text roundWinnerText;
    [Header("End of Game")]
    public GameObject endGameScreen;
    public Text teamWinnerText;

    [Header("Modifiers")]
    public float pointAnimationModifier = 0.1f;
    public float multiplierAnimationModifier = 0.75f;

    private float teamPointsTextStartSize;
    private float teamMultiplierTextStartSize;
    private float clockStartSize;
    private bool textIsScaled = false;
    private Vector3 roundStartSize;
    private PlayerController PlayerController;

    public static SharedUI instance;

    private void Awake() {
        if (!instance) {
            instance = this;
        }
        else {
            Destroy(instance);
            instance = this;
        }
    }

    void Start () {
        teamPointsTextStartSize = team1PointsText.transform.localScale.x;
        teamMultiplierTextStartSize = team1PointsMultiplier.transform.localScale.x;
        clockStartSize = roundTimerText.transform.localScale.x;
        roundStartSize = team1PopObjects.transform.GetChild(0).localScale;
        MultiplierAnimation(1);
        MultiplierAnimation(2);
    }
	
	void Update () {
        UpdateUI();
    }

    public void PointAnimation(int teamID) {

        /*if (rootAngle < 0 && lerpValue > rootAngle)
            lerpValue = lerpValue - 5;
        else if (rootAngle > 0 && lerpValue < rootAngle)
            lerpValue = lerpValue + 5;
        */
        UpdateUI();
        if (teamID == 1) {
            team1PointsText.transform.localScale = new Vector3(teamPointsTextStartSize * 2, teamPointsTextStartSize * 2);
            StartCoroutine(PointAnimation1(team1PointsText));
        }
        else if (teamID == 2) {
            team2PointsText.transform.localScale = new Vector3(teamPointsTextStartSize * 2, teamPointsTextStartSize * 2);
            StartCoroutine(PointAnimation2(team2PointsText));
        }
    }

    public void ShowRoundWinner(int winner) {
        print(winner);
        roundWinnerText.enabled = true;
        if (winner == 1) {
            roundWinnerText.text = "Round Win!";
        } else if (winner == 2) {
            roundWinnerText.text = "Round Lose!";
        } else if (winner == 3) {
            roundWinnerText.text = "Round Tie!";
        } else if (winner == 4) {
            roundWinnerText.text = "Matchpoint Tie!";
        }
    }

    private IEnumerator PointAnimation1(TextMeshProUGUI pointText) {
        while (pointText.transform.localScale.x >= teamPointsTextStartSize) {
            float newValue = pointText.transform.localScale.x - (Time.deltaTime * pointAnimationModifier);
            pointText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }

    private IEnumerator MultiplierAnimation(TextMeshProUGUI multiplierText)
    {
        Debug.Log(multiplierText.text);
        while (multiplierText.transform.localScale.x >= teamMultiplierTextStartSize)
        {
            float newValue = multiplierText.transform.localScale.x - (Time.deltaTime * multiplierAnimationModifier * 2 );
            multiplierText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }
        

        yield return 0;
    }

    private IEnumerator PointAnimation2(TextMeshProUGUI pointText) {
        while (pointText.transform.localScale.x >= teamPointsTextStartSize) {
            float newValue = pointText.transform.localScale.x - (Time.deltaTime * pointAnimationModifier);
            pointText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }

    /*private IEnumerator MultiplierAnimation2(TextMeshProUGUI multiplierText)
    {
        while (multiplierText.transform.localScale.x >= teamMultiplierTextStartSize)
        {
            float newValue = multiplierText.transform.localScale.x - (Time.deltaTime * multiplierAnimationModifier);
            multiplierText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }*/

    public IEnumerator PopRoundWin(int winningTeam) {

        bool animatingRound = true;
        bool gettingBigger = true;
        Vector3 roundEndSize = new Vector3((roundStartSize.x * 3f), (roundStartSize.y * 3f), (roundStartSize.z * 3f));

        GameObject roundsToPop = (winningTeam == 1) ? team1PopObjects : team2PopObjects;
        int numberOfRounds = (winningTeam == 1) ? RoundManager.instance.team1Rounds : RoundManager.instance.team2Rounds;


            while (animatingRound) {
                if (gettingBigger) {
                    
                    roundsToPop.transform.GetChild(numberOfRounds - 1).localScale = Vector3.Lerp(roundsToPop.transform.GetChild(numberOfRounds-1).localScale, roundEndSize, Time.deltaTime * 6);
                    if (roundsToPop.transform.GetChild(numberOfRounds - 1).localScale.x >= roundEndSize.x - 0.01) {
                        gettingBigger = false;
                    }
                    yield return 0;
                }
                else {

                    roundsToPop.transform.GetChild(numberOfRounds - 1).localScale = Vector3.Lerp(roundsToPop.transform.GetChild(numberOfRounds - 1).localScale, roundStartSize, Time.deltaTime * 4);
                if (roundsToPop.transform.GetChild(numberOfRounds - 1).localScale.x <= roundStartSize.x + 0.01) {
                        roundsToPop.transform.GetChild(numberOfRounds - 1).localScale = roundStartSize;
                        //roundsToPop.transform.GetChild(numberOfRounds + 2).localScale = roundStartSize;
                    animatingRound = false;
                    }
                    yield return 0;
                }

            }
        yield return 0;
    }
    private void UpdateKillstreak(float killstreak, TextMeshProUGUI killstreakText, TextMeshProUGUI textToAnimate)
    {
        killstreakText.text = KillMultiplierToText1(killstreak, killstreakText);
        textToAnimate.text = KillMultiplierToText2(killstreak, textToAnimate);
    }

    public string KillMultiplierToText1(float killmultiplier, TextMeshProUGUI killstreakText)
    {
        if (killmultiplier == 1f)
        {
            return "";
        }
        else
        {
            return "Kills: ";
            
        }
    }

    public string KillMultiplierToText2(float killmultiplier, TextMeshProUGUI textToAnimate)
    {
        textToAnimate.text = killmultiplier + "x";
        if (killmultiplier == 1f)
        {
            return "";
        }
        else
        {
            textToAnimate.transform.localScale = new Vector3(teamMultiplierTextStartSize * 1.5f, teamMultiplierTextStartSize * 1.5f);
            StartCoroutine(MultiplierAnimation(textToAnimate));
            //PlayKillstreakAnimation(killstreakText);
            return textToAnimate.text;

        }
    }

    public void MultiplierAnimation(int teamID)
    {
        UpdateUI();
        if (teamID == 1)
        {
            UpdateKillstreak(RoundManager.instance.team1killstreak, team1PointsMultiplier, team1TextToAnimate);
        }

        if (teamID == 2)
        {
            UpdateKillstreak(RoundManager.instance.team2killstreak, team2PointsMultiplier, team2TextToAnimate);
        }
    }
    private void UpdateUI() {
        if (RoundManager.instance.IsOverTime)
        {
            roundTimerText.transform.localScale = new Vector3(0.8f, 0.8f);
            roundTimerText.text = "OVERTIME";
            return;
        }
        if (RoundManager.instance.currentRoundTimer <= 10) {
            roundTimerText.color = Color.red;
            roundTimerText.transform.localScale = new Vector3(clockStartSize * 1.5f, clockStartSize * 1.5f);
            roundTimerText.transform.localScale = new Vector3(clockStartSize - (Time.deltaTime * pointAnimationModifier), clockStartSize - (Time.deltaTime * pointAnimationModifier));
        }
        else {
            roundTimerText.color = Color.white;
            roundTimerText.transform.localScale = new Vector3(clockStartSize, clockStartSize);
        }
        roundTimerText.text = SecondsToMmSs (RoundManager.instance.currentRoundTimer);
        team1PointsText.text = RoundManager.instance.team1Points.ToString();
        team2PointsText.text = RoundManager.instance.team2Points.ToString();
        //UpdateKillstreak(RoundManager.instance.team1killstreak, team1PointsMultiplier);
        //UpdateKillstreak(RoundManager.instance.team2killstreak, team2PointsMultiplier);
        UpdateRoundsWin(RoundManager.instance.team1Rounds, team1PopObjects.transform, team1RoundObjectsBackrounds.transform);
        UpdateRoundsWin(RoundManager.instance.team2Rounds, team2PopObjects.transform, team2RoundObjectsBackrounds.transform);
        startRoundTimerText.text = RoundManager.instance.roundStartTimer.ToString();
    }

    /* private IEnumerator CountdownPulseAnimation()
    {
        int counter = 10;
        while (RoundManager.instance.currentRoundTimer > 0)
        {
            if ()
            roundTimerText.transform.localScale = new Vector3(clockStartSize - (Time.deltaTime * pointAnimationModifier), clockStartSize - (Time.deltaTime * pointAnimationModifier));
        }
        yield return 0;

    } */

    private void UpdateRoundsWin(int roundsWon, Transform parent, Transform backroundParent) {
        for (int i = 0; i < roundsWon; i++) {
            parent.GetChild(i).GetComponent<Image>().enabled = true;
            backroundParent.GetChild(i).GetComponent<Image>().enabled = true;
        }

    }

    private string SecondsToMmSs(int seconds)
    {
        return string.Format("{0}:{1:00}", (seconds / 60) % 60, seconds % 60);
    }

}
