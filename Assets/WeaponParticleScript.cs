using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParticleScript : MonoBehaviour {

    float ChargeModifier = 20f;
    float WeaponCharge;
    ParticleSystem[] particleChildren;
    Transform[] children;
    ParticleSystem shineParticles1, shineParticles2, shineParticles3;


	// Use this for initialization
	void Start () {
        children = GetComponentsInChildren<Transform>();
        particleChildren = GetComponentsInChildren<ParticleSystem>();
        shineParticles1 = particleChildren[1];
        shineParticles2 = particleChildren[2];
        shineParticles3 = particleChildren[3];
        children[1].gameObject.SetActive(!children[1].gameObject.activeSelf);



    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            children[1].gameObject.SetActive(!children[1].gameObject.activeSelf);


        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var spe1 = shineParticles1.emission;
            var spe2 = shineParticles2.emission;
            var spe3 = shineParticles3.emission;
            var spm1 = shineParticles1.main;
            spe1.rateOverTime = 0;
            spe2.rateOverTime = 0;
            spe3.rateOverTime = 0;
            spm1.startSpeed = 1;
            shineParticles1.Emit(1000);
        }
		
	}
}
