using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TABScoreManager : NetworkBehaviour
{
    [SyncVar]
    public int player1TotalScore, player1TotalShots, player1TotalDeaths, player1Accuracy;
    [SyncVar]
    public int player2TotalScore, player2TotalShots, player2TotalDeaths, player2Accuracy;
    public Text player1TotalScoreText, player1TotalShotsText, player1TotalDeathsText, player1AccuracyText;
    public Text player2TotalScoreText, player2TotalShotsText, player2TotalDeathsText, player2AccuracyText;


    public GameObject scoreTable;

    public GameObject crossHair;

    public static TABScoreManager instance;

    void Awake()
    {
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
        ShowOrHideScoretable();
        UpdateScoreBoard();
    }

    private void UpdateScoreBoard()
    {
        player1TotalScoreText.text = player1TotalScore.ToString();
        player2TotalScoreText.text = player2TotalScore.ToString();
        player1AccuracyText.text = (player1Accuracy.ToString() + " %");
        player2AccuracyText.text = (player2Accuracy.ToString() + " %");
        player1TotalShotsText.text = player1TotalShots.ToString();
        player2TotalShotsText.text = player2TotalShots.ToString();
        player1TotalDeathsText.text = player1TotalDeaths.ToString();
        player2TotalDeathsText.text = player2TotalDeaths.ToString();
    }

    #region ServerOperations
    [Command]
    public void CmdShowOrHideScoretabel()
    {
        RpcShowOrHideScoretabel();
    }

    [ClientRpc]
    public void RpcShowOrHideScoretabel()
    {
        ShowOrHideScoretable();
    }
    #endregion
    public void ShowOrHideScoretable()
    {
        //if (isLocalPlayer) {
            if (Input.GetKey(KeyCode.Tab))
            {
                scoreTable.SetActive(true);
                crossHair.SetActive(false);
                UppdateAccuracy();
             }
            else
            {
                scoreTable.SetActive(false);
                crossHair.SetActive(true);
            }
        //}
    }

    public void IncreaseScore(int team, int ammountOfPoints)
    {

        if (team==1)
        {
            player1TotalScore+= ammountOfPoints;
            player1TotalScoreText.text = player1TotalScore.ToString();
        }

        else if (team == 2)
        {
            player2TotalScore+= ammountOfPoints;
            player2TotalScoreText.text = player2TotalScore.ToString();
        }

    }

    private void UppdateAccuracy()
    {
        if (player1TotalShots > 0)
        {
            player1Accuracy  = (int)((float)player2TotalDeaths / player1TotalShots * 100);
            player1AccuracyText.text = (player1Accuracy.ToString() + " %");
        }
 
        if (player2TotalShots > 0) {
            player2Accuracy = (int)((float)player1TotalDeaths / player2TotalShots * 100);
            player2AccuracyText.text = (player2Accuracy.ToString() + " %");
        }         
    }

    public void IncreaseShots(int team)
    {
        if (team == 1)
        {
            player1TotalShots++;
            player1TotalShotsText.text = player1TotalShots.ToString();
        }

        else if (team == 2)
        {
            player2TotalShots++;
            player2TotalShotsText.text = player2TotalShots.ToString();
        }

    }

    public void IncreaseDeaths(int team)
    {

        if (team == 1)
        {
            player1TotalDeaths++;
            player1TotalDeathsText.text = player1TotalDeaths.ToString();
        }

        else if (team == 2)
        {
            player2TotalDeaths++;
            player2TotalDeathsText.text = player2TotalDeaths.ToString();
        }

    }

    public void ResetStats()
    {

        player1TotalScore = 0;
        player1TotalShots = 0;
        player1TotalDeaths = 0;

        player2TotalScore = 0;
        player2TotalShots = 0;
        player2TotalDeaths = 0;

        player1TotalScoreText.text = "0";
        player1TotalShotsText.text = "0";
        player1TotalDeathsText.text = "0";

        player2TotalScoreText.text = "0";
        player2TotalShotsText.text = "0";
        player2TotalDeathsText.text = "0";
    }
}
