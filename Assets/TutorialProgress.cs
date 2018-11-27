using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgress : MonoBehaviour {

    int progress;
    public GameObject MovementRoom, StealthRoom, ShootRoom, DashRoom, DecoyRoom, ObjectivesRoom, MapRoom;
    private float StartTime, LerpTime;
    public AnimationCurve curve;
    public int MovementRoomProgress;


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            if(progress<6)
            progress++;
        }

        if(MovementRoomProgress == 4) {
            progress++;
            MovementRoomProgress++;
            StartTime = Time.time;
        }

        //if(StealthRoomProgress == 5){
        //progress++;
        //StartTime = Time.time;

        //if(ShootyRoomProgress == 6){
        //progress++;
        //shotTargets++;
        //StartTime = Time.time;

        //if(DashRoomProgress == 3){
        //progress++;
        //DashRoomProgress++;
        //StartTime = Time.time;

        //if(DecoyRoomProgress == 2){
        //progress++;
        //DecoyRoomProgress++;
        //StartTime = Time.time;

        //if(ObjectivesRoomProgress == 3){
        //progress++;
        //ObjectivesRoomProgress++;
        //StartTime = Time.time;


        LerpTime = Time.time - StartTime;
        print("Start Time: "+ StartTime + " LerpTime: " + LerpTime);
 
        switch(progress) {
            case 1:
            MovementRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 2:
            StealthRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 3:
            ShootRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 4:
            DashRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 5:
            DecoyRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 6:
            ObjectivesRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
        }
		
	}
}
