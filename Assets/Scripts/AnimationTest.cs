using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

    public Animator animator;
    public float chargeSpeed = 0.01f;
    public float speed=0;

    // Use this for initialization
    void Start () {
        animator.SetFloat("Velocity", 0);

    }
	
	// Update is called once per frame
	void Update () {


        ChangeStance();

        if (Input.GetKey("space"))
        {
            animator.SetFloat("Velocity", 1);
        }

        else
        {
            animator.SetFloat("Velocity", 0);
        }       
    
    }

    private void ChangeStance()
    {
        if (Input.GetKey("w"))
        {
            if(speed<1)
                speed = speed + chargeSpeed;

            animator.SetLayerWeight(1, speed);//tänk på hur många layers det finns
            Debug.Log(speed);
        }
        else if (Input.GetKey("s"))
        {
            if (speed > 0)
                speed = speed - chargeSpeed;

            animator.SetLayerWeight(1, speed);
            Debug.Log(speed);
        }
    }


}


