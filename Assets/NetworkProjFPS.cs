using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkProjFPS : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    // Use this for initialization
    void Start() {
        if (!isLocalPlayer) {
            foreach (Behaviour component in componentsToDisable) {
                component.enabled = false;
            }
        }
    }

    public bool CheckIfLocal() {
        return isLocalPlayer;
    }
}
