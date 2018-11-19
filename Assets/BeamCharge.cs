using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCharge : MonoBehaviour {

    PlayerController pc;
    ParticleSystem ps;
    float Emission;

    // Use this for initialization
    void Start () {
        pc = GetComponentInParent<PlayerController>();
        print(pc);
        ps = GetComponentInChildren<ParticleSystem>();
        print(ps);
        
		
	}
	
	// Update is called once per frame
	void Update () {

        print(pc.beamDistance);
        var temp = ps.emission;
        temp.rateOverTime = Mathf.Clamp(pc.beamDistance*2, 0, 60);
		
	}
}
