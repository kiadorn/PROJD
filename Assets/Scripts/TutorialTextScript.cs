﻿using System.Collections;
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

        RoomInfo = new string[] {  "•Let's start off with some <b>movement!</b>\n\n•Use <b>WASD</b> to move.\n\n•Use <b>Space</b> to <b>jump</b>.",
                                        "•You are stealthed when standing on the colour of your team\n\n•You are visible when standing on the colour of the enemy team or on neutral ground.",
                                        "•Press <b>Left Mouse Button</b> to <b>shoot.</b>\n\n•<b>Hold to charge</b> your laser, <b>increasing the range</b> of it.\n\n•You can press <b>Right Mouse Button</b> to <b>cancel</b> your laser.\n\n•Hitting an enemy gives you <b>100 points</b>.",
                                        "•Press <b>Left Shift</b> to <b>dash</b>.\n\n•You can use it either on the ground or in the air.",
                                        "•Press <b>E</b> to use your <b>decoy</b>.\n\n•It will start running in the direction you are facing.\n\n•It will run for 3 seconds.",
                                        "•<b>Collecting orbs</b> give you <b>points</b>.\n\n•Your own coloured orbs and smaller neutral orbs give you <b>50 points</b>.\n\n•The enemy's orbs give <b>100 points</b>.\n\n•The big orb in the center of the room gives you <b>150 points</b>.",
                                        "•Here is an overview of the map!" };


    }
	
	void Update () {
        leftText.text = RoomInfo[currentID];
        updateProgressText();
        rightText.text = RoomInstructions[currentID];
    }

    private void updateProgressText()
    {

        RoomInstructions = new string[] {  "Tiles stepped on: " + tp.MovementRoomProgress +"/4",
                                                "Seconds spent in stealth: " + (int)tp.StealthRoomProgress +"/5",
                                                "Targets hit: " + tp.ShootyRoomProgress +"/6",
                                                "Platforms passed: " + tp.DashRoomProgress +"/3",
                                                "Decoy used: " + tp.DecoyRoomProgress +"/1",
                                                "Points collected: " + tp.ObjectivesRoomProgress +"/1200",
                                                "Stand on the green platform to return to the menu", };
    }
}
