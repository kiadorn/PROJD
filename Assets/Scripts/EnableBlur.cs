using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;
using UnityEngine.Networking;
using UnityStandardAssets.ImageEffects;

public class EnableBlur : MonoBehaviour {

    private bool blurEnabled = false;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            blurEnabled = !blurEnabled;
            EnableOrDisable(blurEnabled);
        }
    }


    private void EnableOrDisable(bool b) {
        UIEffect[] blursToEnable = GetComponentsInChildren<UIEffect>();
        foreach(UIEffect blur in blursToEnable) {
            blur.enabled = b;
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                player.GetComponent<PlayerController>().cam.GetComponent<BlurOptimized>().enabled = b;
                player.GetComponent<PlayerController>().cam.GetComponent<EdgeDetection>().sampleDist = b ? 5 : 1;
            }
        }
    }
}
