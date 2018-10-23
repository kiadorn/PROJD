using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLaser : MonoBehaviour { //obselet kod

    public PlayerController controller;
    public GameObject laser;

    private float forwardStartspeed;

    public float _beamDistance = 0f;

    // Use this for initialization
    void Start () {
        controller = gameObject.GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("z"))
        {
            laser.SetActive(true);
        }
        if (Input.GetKeyDown("c"))
        {
            laser.SetActive(false);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if(_beamDistance<5.0)
                _beamDistance += Time.deltaTime*0.5f;

            laser.transform.localScale += new Vector3(0, _beamDistance, 0);
            laser.transform.localPosition += new Vector3(0, 0, _beamDistance);
        }
        else
        {
            _beamDistance = 0f;
        }
        if(Input.GetKeyUp("p")) 
        {
            //_beamDistance = 1f;
            laser.transform.localScale += new Vector3(0, _beamDistance, 0);
            laser.transform.localPosition += new Vector3(0, 0, _beamDistance);
        }

    }
}
