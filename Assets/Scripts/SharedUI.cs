﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SharedUI : MonoBehaviour {

    [Header("Start of Round")]
    public Text startRoundTimerText;
    [Header("During Rounds")]
    public Text roundTimerText;
    public Text team1PointsText;
    public Text team2PointsText;
    public GameObject team1RoundObjects;
    public GameObject team2RoundObjects;
    [Header("End of Round")]
    public Text roundWinnerText;
    [Header("End of Game")]
    public GameObject endGameScreen;
    public Text teamWinnerText;

    [Header("Modifiers")]
    public float pointAnimationModifier = 0.1f;

    private float teamPointsTextStartSize;
    private float clockStartSize;
    private Vector3 roundStartSize;

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
        clockStartSize = roundTimerText.transform.localScale.x;
        roundStartSize = team1RoundObjects.transform.GetChild(0).localScale;
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
        roundWinnerText.enabled = true;
        if (winner == 1) {
            roundWinnerText.text = "You won the round!";
        } else if (winner == 2) {
            roundWinnerText.text = "You lost the round!";
        } else if (winner == 3) {
            roundWinnerText.text = "It's a TIE!";
        }
    }

    private IEnumerator PointAnimation1(Text pointText) {
        while (pointText.transform.localScale.x >= teamPointsTextStartSize) {
            float newValue = pointText.transform.localScale.x - (Time.deltaTime * pointAnimationModifier);
            pointText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }

    private IEnumerator PointAnimation2(Text pointText) {
        while (pointText.transform.localScale.x >= teamPointsTextStartSize) {
            float newValue = pointText.transform.localScale.x - (Time.deltaTime * pointAnimationModifier);
            pointText.transform.localScale = new Vector3(newValue, newValue);

            yield return 0;
        }


        yield return 0;
    }

    public IEnumerator PopRoundWin(int winningTeam) {

        bool animatingRound = true;
        bool gettingBigger = true;
        Vector3 roundEndSize = new Vector3((roundStartSize.x * 1.5f), (roundStartSize.y * 1.5f), (roundStartSize.z * 1.5f));

        GameObject roundsToPop = (winningTeam == 1) ? team1RoundObjects : team2RoundObjects;
        int numberOfRounds = (winningTeam == 1) ? RoundManager.instance.team1Rounds : RoundManager.instance.team2Rounds;


            while (animatingRound) {
                if (gettingBigger) {
                    
                    roundsToPop.transform.GetChild(numberOfRounds - 1).localScale = Vector3.Lerp(roundsToPop.transform.GetChild(numberOfRounds-1).localScale, roundEndSize, Time.deltaTime * 6);
                    roundsToPop.transform.GetChild(numberOfRounds + 2).localScale = Vector3.Lerp(roundsToPop.transform.GetChild(numberOfRounds + 2).localScale, roundEndSize, Time.deltaTime * 6);
                    if (roundsToPop.transform.GetChild(numberOfRounds - 1).localScale.x >= roundEndSize.x - 0.01) {
                        gettingBigger = false;
                    }
                    yield return 0;
                }
                else {

                    roundsToPop.transform.GetChild(numberOfRounds - 1).localScale = Vector3.Lerp(roundsToPop.transform.GetChild(numberOfRounds - 1).localScale, roundStartSize, Time.deltaTime * 4);
                    roundsToPop.transform.GetChild(numberOfRounds + 2).localScale = Vector3.Lerp(roundsToPop.transform.GetChild(numberOfRounds + 2).localScale, roundStartSize, Time.deltaTime * 4);
                if (roundsToPop.transform.GetChild(numberOfRounds - 1).localScale.x <= roundStartSize.x + 0.01) {
                        roundsToPop.transform.GetChild(numberOfRounds - 1).localScale = roundStartSize;
                        roundsToPop.transform.GetChild(numberOfRounds + 2).localScale = roundStartSize;
                    animatingRound = false;
                    }
                    yield return 0;
                }

            }
        yield return 0;
    }

    private void UpdateUI() {
        if (RoundManager.instance.currentRoundTimer <= 10) {
            roundTimerText.color = Color.red;
            roundTimerText.transform.localScale = new Vector3(clockStartSize * 1.5f, clockStartSize * 1.5f);
        }
        else {
            roundTimerText.color = Color.white;
            roundTimerText.transform.localScale = new Vector3(clockStartSize, clockStartSize);
        }
        roundTimerText.text = RoundManager.instance.currentRoundTimer.ToString();
        team1PointsText.text = RoundManager.instance.team1Points.ToString();
        team2PointsText.text = RoundManager.instance.team2Points.ToString();
        UpdateRoundsWin(RoundManager.instance.team1Rounds, team1RoundObjects.transform);
        UpdateRoundsWin(RoundManager.instance.team2Rounds, team2RoundObjects.transform);
        startRoundTimerText.text = RoundManager.instance.roundStartTimer.ToString();
    }

    private void UpdateRoundsWin(int roundsWon, Transform parent) {
        for (int i = 0; i < roundsWon; i++) {
            parent.GetChild(i).GetComponent<Image>().enabled = true;
        }
    }



}
