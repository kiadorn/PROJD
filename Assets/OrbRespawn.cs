using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbRespawn : MonoBehaviour {

    public GameObject Orb;
    public AnimationCurve orbSize;

    void Start() {
        Orb.SetActive(true);

    }

    private void Update() {
        Orb.transform.localScale = new Vector3(orbSize.Evaluate(Time.time/15), orbSize.Evaluate(Time.time/15), orbSize.Evaluate(Time.time/15));
    }

}
