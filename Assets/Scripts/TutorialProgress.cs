using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgress : MonoBehaviour {

    public MaterialSwap ms;
    public int progress;
    public GameObject MovementRoom, StealthRoom, ShootyRoom, DashRoom, DecoyRoom, ObjectivesRoom, MapRoom;
    private float StartTime, LerpTime;
    public AnimationCurve curve;
    public AnimationCurve Bridgecurve;
    public int MovementRoomProgress, DashRoomProgress, ShootyRoomProgress, DecoyRoomProgress, ObjectivesRoomProgress;
    public float StealthRoomProgress;
    private bool _movementDone, _shootyDone, _dashDone, _decoyDone, _objectiveDone;

	
	// Update is called once per frame
	void Update () {
        if(MovementRoomProgress == 4 && !_movementDone) {
            progress++;
            _movementDone = true;
            SoundManager.instance.PlayTutorialNextRoom(GameObject.Find("MovementPlaySoundFromHere"));
            StartTime = Time.time;
        }
        if (RoundManager.instance != null && ObjectivesRoomProgress != 1200) ObjectivesRoomProgress = RoundManager.instance.team1Points;

        if (progress == 1) {
            if(ms == null) {
                ms = GameObject.FindGameObjectWithTag("Player").GetComponent<MaterialSwap>();
                
            }
            if (ms.isVisible == true) 
            {
                StealthRoomProgress = 0;
            }
            else 
            {
                StealthRoomProgress += Time.deltaTime;
            }
            if(StealthRoomProgress >= 5) 
            {
                progress++;
                SoundManager.instance.PlayTutorialNextRoom(GameObject.Find("StealthPlaySoundFromHere"));
                StartTime = Time.time;
            }
        }

        if(ShootyRoomProgress == 6 && !_shootyDone) {
            progress++;
            _shootyDone = true;
            SoundManager.instance.PlayTutorialNextRoom(GameObject.Find("ShootyPlaySoundFromHere"));
            StartTime = Time.time;
        }

        if(DashRoomProgress == 3 && !_dashDone) {
            progress++;
            _dashDone = true;
            SoundManager.instance.PlayTutorialNextRoom(GameObject.Find("DashPlaySoundFromHere"));
            StartTime = Time.time;
        }

        if(DecoyRoomProgress == 1 && !_decoyDone) {
            progress++;
            _decoyDone = true;
            SoundManager.instance.PlayTutorialNextRoom(GameObject.Find("DecoyPlaySoundFromHere"));
            StartTime = Time.time;
            
        }

        if(RoundManager.instance.team1Points >= 1200 && !_objectiveDone) {
            progress++;
            _objectiveDone = true;
            SoundManager.instance.PlayTutorialNextRoom(GameObject.Find("ObjectivePlaySoundFromHere"));
            StartTime = Time.time;
        }


        LerpTime = Time.time - StartTime;
        //print("Start Time: "+ StartTime + " LerpTime: " + LerpTime);
 
        switch(progress) {
            case 1:
            MovementRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 2:
            StealthRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            break;
            case 3:
            ShootyRoom.transform.Find("Front Door").localPosition = new Vector3(0, curve.Evaluate(LerpTime), 12.5f);
            ShootyRoom.transform.Find("Bridge").localPosition = new Vector3(0, Bridgecurve.Evaluate(LerpTime), 3.329f);
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
