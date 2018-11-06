using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;

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
    }
}
