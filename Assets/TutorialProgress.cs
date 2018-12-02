using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgress : MonoBehaviour {

    public MaterialSwap ms;
    public int progress;
    public GameObject MovementRoom, StealthRoom, ShootyRoom, DashRoom, DecoyRoom, ObjectivesRoom, MapRoom;
    private float StartTime, LerpTime;
    public AnimationCurve curve;
    public int MovementRoomProgress, DashRoomProgress, ShootyRoomProgress, DecoyRoomProgress;
    public float StealthRoomProgress;


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

        if(ShootyRoomProgress == 6) {
            progress++;
            ShootyRoomProgress++;
            StartTime = Time.time;
        }

        if(DashRoomProgress == 3) {
            progress++;
            DashRoomProgress++;
            StartTime = Time.time;
        }
        print(DecoyRoomProgress);
        if(DecoyRoomProgress == 1) {
            progress++;
            DecoyRoomProgress++;
            StartTime = Time.time;
        }

        //if(ObjectivesRoomProgress == 3){
        //progress++;
        //ObjectivesRoomProgress++;
        //StartTime = Time.time;


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
