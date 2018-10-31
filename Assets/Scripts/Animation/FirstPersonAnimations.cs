using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonAnimations : MonoBehaviour {

    private Animator animator;

    public bool fired = false;

    // Use this for initialization
    void Start () {

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        if (!(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")))
        {
            animator.SetFloat("Velocity", 0);
            animator.speed = 1.0f;

            //spineY = 0;
            //spineY = -spineY;

        }
        else if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            animator.SetFloat("Velocity", 1);
            animator.speed = 1.0f;
        }

        else if ((Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))&&Input.GetKey("z"))
        {


            animator.SetFloat("Velocity", 1);
            animator.speed = 5.0f;
        }
        if (Input.GetKey("f"))
        {

            animator.SetBool("Fire", false);
            animator.SetBool("Charge",true);
            //animator.speed = 5.0f;
        }
        if (Input.GetKey("g"))
        {

            animator.SetBool("Charge", false);
            animator.SetBool("Fire",true);
            //animator.speed = 5.0f;
        }
        if (Input.GetKey("j"))
        {

            animator.SetBool("Land", false);
            animator.SetBool("Jump", true);
            //animator.speed = 5.0f;
        }
        if (Input.GetKey("l"))
        {

            animator.SetBool("Jump", false);
            animator.SetBool("Land", true);
            //animator.speed = 5.0f;
        }

    }
}
