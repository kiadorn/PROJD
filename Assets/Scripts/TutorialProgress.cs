using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgress : MonoBehaviour {

    public MaterialSwap ms;
    public int progress;
    public GameObject MovementRoom, StealthRoom, ShootyRoom, DashRoom, DecoyRoom, ObjectivesRoom, MapRoom;
    private float StartTime, LerpTime;
    public AnimationCurve curve;
    public int MovementRoomProgress, DashRoomProgress, ShootyRoomProgress, DecoyRoomProgress, ObjectivesRoomProgress;
    public float StealthRoomProgress;
    private bool _movementDone, _shootyDone, _dashDone, _decoyDone, _objectiveDone;

	
	// Update is called once per frame
	void Update () {
        if(MovementRoomProgress == 4 && !_movementDone) {
            progress++;
            _movementDone = true;
            StartTime = Time.time;
        }
        if (RoundManager.instance != null && ObjectivesRoomProgress != 1600) ObjectivesRoomProgress = RoundManager.instance.team1Points;

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
                StartTime = Time.time;
            }
        }

        if(ShootyRoomProgress == 6 && !_shootyDone) {
            progress++;
            _shootyDone = true;
            StartTime = Time.time;
        }

        if(DashRoomProgress == 3 && !_dashDone) {
            progress++;
            _dashDone = true;
            StartTime = Time.time;
        }

        if(DecoyRoomProgress == 1 && !_decoyDone) {
            progress++;
            _decoyDone = true;
            StartTime = Time.time;
            
        }

        if(RoundManager.instance.team1Points >= 1500 && !_objectiveDone) {
            progress++;
            _objectiveDone = true;
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
