using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeClasses : MonoBehaviour {

    public GameObject SoldierMask;
    public GameObject SorcererMask;
    public GameObject ScoutrMask;
    

    public float speedMultyplier = 1f;

    public PlayerController controller;

    private float forwardStartspeed;
    private float backwardStartspeed;
    private float strafeStartspeed;  

    // Use this for initialization
    void Start () {
        controller = gameObject.GetComponent<PlayerController>();
        forwardStartspeed = controller.movementSettings.forwardSpeed;
        backwardStartspeed = controller.movementSettings.backwardSpeed;
        strafeStartspeed = controller.movementSettings.strafeSpeed;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("t"))
        {
            speedMultyplier = 1f;
            SoldierMask.SetActive(true);
            SorcererMask.SetActive(false);
            ScoutrMask.SetActive(false);
        }
        else if (Input.GetKeyDown("y"))
        {
            speedMultyplier = 0.9f;
            SoldierMask.SetActive(false);
            SorcererMask.SetActive(true);
            ScoutrMask.SetActive(false);
        }
        else if (Input.GetKeyDown("u"))
        {
            speedMultyplier = 1f;
            SoldierMask.SetActive(false);
            SorcererMask.SetActive(false);
            ScoutrMask.SetActive(true);
        }

        ClassStats();

    }

    public void ClassStats()
    {
        controller.movementSettings.forwardSpeed = forwardStartspeed * speedMultyplier;
        controller.movementSettings.backwardSpeed = backwardStartspeed * speedMultyplier;
        controller.movementSettings.strafeSpeed = strafeStartspeed * speedMultyplier;
        
    }

}
