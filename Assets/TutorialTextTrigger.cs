using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextTrigger : MonoBehaviour {

    TutorialTextScript tts;
    public int id;

    private void Update() {
        if(tts == null && GameObject.Find("Tutorial Text") != null) tts = GameObject.Find("Tutorial Text").GetComponent<TutorialTextScript>();
    }

    private void OnTriggerEnter(Collider other) {
        if(tts != null) tts.currentID = id;
        tts.InfoText.SetActive(true);
    }
}
