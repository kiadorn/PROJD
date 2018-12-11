using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasScript : MonoBehaviour {
    
    public GameObject snowParticles; 
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {


        if (Input.GetKeyDown(KeyCode.J))
        {
            snowParticles.SetActive(!snowParticles.activeSelf);
        }
    }


}
