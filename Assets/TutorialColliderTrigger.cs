using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialColliderTrigger : MonoBehaviour {

    TutorialProgress progress;
    public Material ClearedMaterial;
    bool triggered = false;

    private void Start() {
        progress = GameObject.Find("Tutorial Manager").GetComponent<TutorialProgress>();
    }

    private void OnCollisionEnter(Collision collision) {
        if(!triggered) {
            triggered = true;
            GetComponent<MeshRenderer>().material = ClearedMaterial;
            if(progress.MovementRoomProgress < 4) progress.MovementRoomProgress++;
            
        }
        
    }
}
