using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour {

    public float speedMultiplier;
    float timer;1

    MeshRenderer meshr;
    bool fadeOut = false;
    bool fadeIn = false;

	// Use this for initialization
	void Start () {
        meshr = gameObject.GetComponent<MeshRenderer>();
        timer = meshr.material.GetFloat("Vector1_6C82E8EC");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Alpha1)){
            timer = 1;
            meshr.material.SetFloat("Vector1_6C82E8EC", timer);
            fadeIn = true;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            timer = -1;
            meshr.material.SetFloat("Vector1_6C82E8EC", timer);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            timer = -1;
            meshr.material.SetFloat("Vector1_6C82E8EC", timer);
            fadeOut = true;
        }

        if (fadeIn == true)
        {
            if (timer > -1)
            {
                timer -= Time.deltaTime*speedMultiplier;
                meshr.material.SetFloat("Vector1_6C82E8EC", timer);
            }
            else fadeIn = false;
        }

        if (fadeOut == true)
        {
            if (timer < 1)
            {
                timer += Time.deltaTime*speedMultiplier;
                meshr.material.SetFloat("Vector1_6C82E8EC", timer);
            }
            else fadeOut = false;
        }
        //("Vector1_6C82E8EC")
    }
}
