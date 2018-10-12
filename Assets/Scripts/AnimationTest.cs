using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {

    public Animator animator;
    public float chargeSpeed = 0.01f;
    public float speed=0;

    public bool fiered=false;
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

        if (Input.GetKeyDown("j"))
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Land", false);
        }

        if (Input.GetKey("l"))
        {
            animator.SetBool("Land", true);
        }

        if (Input.GetKeyDown("d"))
        {
            animator.SetBool("Death", true);
        }


    }

    private void ChangeStance()
    {

        if (Input.GetKey("w"))
        {
            animator.SetBool("Fire", false);
            fiered = false;

            if (speed<1)
                speed = speed + chargeSpeed;

            animator.SetLayerWeight(1, speed);//tänk på hur många layers det finns
            Debug.Log(speed);
        }
        else //if (Input.GetKey("s"))
        {
            animator.SetBool("Fire", true);
               


            if (fiered&& animator.GetCurrentAnimatorStateInfo(1).IsName("Charge"))
            {

                if (speed > 0)
                    speed = speed - (chargeSpeed * 5);
                
                animator.SetBool("Fire", false);
                //animator.SetBool("Fire", false);
                //speed = 0;

                animator.SetLayerWeight(1, speed);
                
                
                /*
                if (speed > 0)
                    speed = speed - (chargeSpeed*5);
                else
                    animator.SetBool("Fire", false);

                animator.SetLayerWeight(1, speed);
                */
                //fiered = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(1).IsName("Fire"))
            {
                fiered = true;
            }


            //if (speed > 0)
             //   speed = speed - chargeSpeed;

            //animator.SetLayerWeight(1, speed);
            //Debug.Log(speed);
        }
    }


}


