using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextTrigger : MonoBehaviour {

    public TutorialTextScript tts;
    public int id;

    private void Start() {
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player"))
        {
            tts.currentID = id;
            tts.InfoText.SetActive(true);

        }
    }
}
