using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoySpawn : MonoBehaviour {

    public GameObject decoy;
    private GameObject newDecoy;


    // Use this for initialization
    void Start () {
    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("f")) { 
            newDecoy = Instantiate(decoy) as GameObject;
            Destroy(newDecoy, 5f);
            newDecoy.transform.rotation = transform.rotation;

            //newDecoy.animator.SetFloat("Velocity", 1);
        }
    }

}
