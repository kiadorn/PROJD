using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgress : MonoBehaviour {

    int progress;
    public GameObject MovementRoom, StealthRoom, ShootRoom, DashRoom, DecoyRoom, ObjectivesRoom, MapRoom;


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            progress++;
        }
        print(MovementRoom.transform.Find("Front Wall").transform.Find("Front Door").localPosition);
        switch(progress) {
            case 1: MovementRoom.transform.Find("Front Door").transform.Find("Front Door").localPosition = new Vector3(0, -10, 0);
            break;
        }
		
	}
}
