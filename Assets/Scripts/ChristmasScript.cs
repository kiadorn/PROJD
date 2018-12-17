using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasScript : MonoBehaviour {

    private bool christmasMode = false;
    public GameObject snowParticles;
    public Material OrangeSnowMat, PurpleSnowMat, OrangeBaseMat, PurpleBaseMat, ParticleSmonk, ParticleSnow;
    public Renderer OrangeOuter, OrangeBox1, OrangeBox2, OrangeCover, OrangePlatform, OrangeRamp, PurpleOuter, PurpleBox1, PurpleBox2, PurpleCover, PurplePlatform, PurpleRamp;
    private Renderer[] OrangeRenderers, PurpleRenderers;
	void Start () {
		OrangeRenderers = new Renderer[] { OrangeOuter, OrangeBox1, OrangeBox2, OrangeCover, OrangePlatform, OrangeRamp};
        PurpleRenderers = new Renderer[] { PurpleOuter, PurpleBox1, PurpleBox2, PurpleCover, PurplePlatform, PurpleRamp};

        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Christmas")) {
            go.SetActive(false);
        }
    }



    void Update() {


        if (Input.GetKeyDown(KeyCode.J))
        {
            Christmas();
        }
    }

    void Christmas() {
        christmasMode = !christmasMode;
        snowParticles.SetActive(!snowParticles.activeSelf);
        if(christmasMode) {
            foreach(Renderer r in OrangeRenderers) {
                r.material = OrangeSnowMat;
            }
            foreach(Renderer r in PurpleRenderers) {
                r.material = PurpleSnowMat;
            }
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
                go.transform.Find("InvisibleTrail").GetComponent<ParticleSystem>().enableEmission = false;
                go.transform.Find("InvisibleSnow").GetComponent<ParticleSystem>().enableEmission = true;
            }
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Christmas")) {
                go.SetActive(true);
            }
        }
        else {
            foreach(Renderer r in OrangeRenderers) {
                r.material = OrangeBaseMat;
            }
            foreach(Renderer r in PurpleRenderers) {
                r.material = PurpleBaseMat;
            }
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
                go.transform.Find("InvisibleTrail").GetComponent<ParticleSystem>().enableEmission = true;
                go.transform.Find("InvisibleSnow").GetComponent<ParticleSystem>().enableEmission = false;
            }
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Christmas")) {
                go.SetActive(false);
            }
        }
    }
}
