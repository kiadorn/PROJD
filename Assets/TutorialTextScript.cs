using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialTextScript : MonoBehaviour {

    public int currentID;
    TextMeshProUGUI leftText, rightText;
    public GameObject InfoText;
    TutorialProgress tp;
    string[] RoomInfo, RoomInstructions;


    void Start() {
        tp = GameObject.Find("Tutorial Manager").GetComponent<TutorialProgress>();
        InfoText = GameObject.Find("LeftText");
        leftText = transform.Find("LeftText").GetComponent<TextMeshProUGUI>();
        rightText = transform.Find("RightText").GetComponent<TextMeshProUGUI>();


        RoomInfo = new string[] {  "Let's start off with some movement, use WASD to move, and Space to jump",
                                        "You are stealthed when standing on the colour of your team, but you are visible when standing on the colour of the enemy team or on neutral ground",
                                        "Press Left Mouse Button to shoot, hold to charge your laser, increasing the range of it. You can press Right Mouse Button to cancel your laser. Your move slower while charging. Hitting an enemy gives you 1 point",
                                        "Press Left Shift to dash, you can use it either on the ground or in the air",
                                        "Press E to use your decoy, it will start running in the direction you are facing and will run for 3 seconds",
                                        "Collecting orbs gives you points, collecting your own coloured orbs gives you one point, the enemy's orbs give you 2 points, the big orb in the center of the room gives you 3 points, and smaller neutral orbs give you 1 point",
                                        "Here is an overview of the map" };
        RoomInstructions = new string[] {  "Tiles stepped on: " + tp.MovementRoomProgress +"/4",
                                                "Seconds spent in stealth: " + tp.StealthRoomProgress +"/5",
                                                "Targets hit: " + tp.ShootyRoomProgress +"/6",
                                                "Platforms passed: " + tp.DashRoomProgress +"/3",
                                                "Decoy used: " + tp.DecoyRoomProgress +"/1",
                                                "Points collected: " + tp.ObjectivesRoomProgress +"/5",
                                                "Stand on the green platform to return to the menu", };

    }
	
	void Update () {
        leftText.text = RoomInfo[currentID];
        rightText.text = RoomInstructions[currentID];

        if(Input.GetKeyDown(KeyCode.T)) {
            InfoText.SetActive(!InfoText.activeSelf);
        }
    }
}
