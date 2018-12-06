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
    public RoundManager rm;
    private bool _movementDone, _shootyDone, _dashDone, _decoyDone, _objectiveDone;

    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if(rm == null && GameObject.Find("RoundManager") != null) rm = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        if(rm != null && ObjectivesRoomProgress != 16) ObjectivesRoomProgress = rm.team1Points;
        

        if(MovementRoomProgress == 4 && !_movementDone) {
            progress++;
            _movementDone = true;
            StartTime = Time.time;
        }
        
        if(progress == 1) {
            if(ms == null) {
                ms = GameObject.FindGameObjectWithTag("Player").GetComponent<MaterialSwap>();
                
            }
            StartCoroutine(InvisTimer());
            if(ms.isVisible == true) StealthRoomProgress = 0;
            if(StealthRoomProgress > 5) {
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

        if(ObjectivesRoomProgress == 15 && !_objectiveDone) {
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

    private IEnumerator InvisTimer() {
        while(ms.isVisible == false) {
            StealthRoomProgress += Time.deltaTime/60;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return 0;
    }
}
