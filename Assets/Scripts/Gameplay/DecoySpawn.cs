using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoySpawn : MonoBehaviour {

    public GameObject decoy;
    private GameObject newDecoy;
    public float destructionTime = 2f;
    public PlayerController controller;

    // Use this for initialization
    void Start () {
    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("f")) { 
            newDecoy = Instantiate(decoy) as GameObject;
            Destroy(newDecoy, destructionTime);
            newDecoy.transform.rotation = transform.rotation;
            newDecoy.GetComponent<DecoyBehaviour>().controller = controller;
            newDecoy.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y-1, transform.localPosition.z);
            

            //newDecoy.animator.SetFloat("Velocity", 1);
        }
    }

}
