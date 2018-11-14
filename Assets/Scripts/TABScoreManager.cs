using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TABScoreManager : NetworkBehaviour
{
    [SyncVar]
    public int player1Score, player1Shots, player1Deaths;
    [SyncVar]
    public int player2Score, player2Shots, player2Deaths;

    public Text player1ScoreText, player1ShotsText, player1DeathsText;
    public Text player2ScoreText, player2ShotsText, player2DeathsText;

    public GameObject scoreTabel;

    public GameObject crossHair;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
        ShowOrHideScoretabel();

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
        ShowOrHideScoretabel();
    }
    #endregion
    public void ShowOrHideScoretabel()
    {
        //if (isLocalPlayer) {
            if (Input.GetKey(KeyCode.Tab))
            {
                scoreTabel.SetActive(true);
                crossHair.SetActive(false);
            }
            else
            {
                scoreTabel.SetActive(false);
                crossHair.SetActive(true);
            }
        //}
    }

    public void IncreaseScore(int teamID)
    {

        if (teamID==1)
        {
            player1Score++;
            player1ScoreText.text = player1Score.ToString();
        }

        else if (teamID == 2)
        {
            player2Score++;
            player2ScoreText.text = player2Score.ToString();
        }

    }

    public void IncreaseShots(int teamID)
    {

        if (teamID == 1)
        {
            player1Shots++;
            player1ShotsText.text = player1Shots.ToString();
        }

        else if (teamID == 2)
        {
            player2Shots++;
            player2ShotsText.text = player2Shots.ToString();
        }

    }

    public void IncreaseDeaths(int teamID)
    {

        if (teamID == 1)
        {
            player1Deaths++;
            player1DeathsText.text = player1Deaths.ToString();
        }

        else if (teamID == 2)
        {
            player2Deaths++;
            player2DeathsText.text = player2Deaths.ToString();
        }

    }

    public void ResetStats()
    {

        player1Score = 0;
        player1Shots = 0;
        player1Deaths=0;

        player2Score = 0;
        player2Shots = 0;
        player2Deaths = 0;

        player1ScoreText.text = "0";
        player1ShotsText.text = "0";
        player1DeathsText.text = "0";

        player2ScoreText.text = "0";
        player2ShotsText.text = "0";
        player2DeathsText.text = "0";
    }

}
