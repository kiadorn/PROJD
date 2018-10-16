using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AnimationTest : MonoBehaviour {

    public Animator animator;
    public float chargeSpeed = 0.01f;
    public float speed = 0;
    public float chargeStartWeight = 0.5f;

    public GameObject parentScript;

    public CharacterFade test;

    public Transform spine;

    public float rotationSpeed = 100f;

    public bool fiered = false;

    public Animation runningAnim;

    // Use this for initialization
    void Start() {
        animator.SetFloat("Velocity", 0);
        animator.SetBool("Fire", false);
        characterRotation = this.transform;
        //characterYStart = characterRotation.rotation.y;
        Debug.Log(characterYStart);
    }

    public Transform characterRotation;

    float spineY;
    float spineZ;
    float characterY;
    public float characterYStart;
    // Update is called once per frame
    void Update() {


        ChangeStance();

        if (Input.GetKey("w"))
        {
            animator.SetFloat("Velocity", 1);
            
        }


        else
        {
            animator.SetFloat("Velocity", 0);
            
        }

        if(Input.GetKey("w")&& Input.GetKey(KeyCode.LeftShift))
        {
            animator.speed = 5.0f;
        }
        else
        {
            animator.speed = 1.0f;
        }

        if (parentScript.GetComponent<RigidbodyFirstPersonController>().Jumping)
        {
            animator.SetBool("Jump", true);
            animator.SetBool("Land", false);
        }

        else if (parentScript.GetComponent<RigidbodyFirstPersonController>().Grounded)
        {
            animator.SetBool("Land", true);
        }

        if (Input.GetKeyDown("k"))
        {
            animator.SetBool("Death", true);
            spineZ = 0;
            spineY = 0;
        }

        UppdateSpineRotation();
        UppdateMovemtRotation();
    }

    void LateUpdate()
    {



        spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + spineY, spine.eulerAngles.z + spineZ);

       
    }

    private void UppdateSpineRotation(){

        if (Input.GetKey("up"))
        {
            spineZ = spineZ - Time.deltaTime * rotationSpeed;
        }
        else if (Input.GetKey("down"))
        {
            spineZ = spineZ + Time.deltaTime * rotationSpeed;
        }

        if (Input.GetKey("left"))
        {
            spineY = spineY - Time.deltaTime * rotationSpeed;
        }
        else if (Input.GetKey("right"))
        {
            spineY = spineY + Time.deltaTime * rotationSpeed;
        }
    }
   
    private void UppdateMovemtRotation()
    {
        

        if (Input.GetKey("a"))
        {
            //characterY = characterYStart - 90;
        }
        else if (Input.GetKey("d"))
        {
            //characterY = characterYStart + 90;
        }
        else
        {
            //characterY = characterYStart;
        }
        
        //characterRotation.transform.rotation = Quaternion.Euler(0, characterY, 0);
    }


    private void ChangeStance()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {//borde göra så att övergången till chargen blir långamare ju mer tid som har gått
            speed = chargeStartWeight;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            animator.SetBool("Fire", false);
            fiered = false;

            
            if (speed<1)
                speed = speed + chargeSpeed;

            animator.SetLayerWeight(1, speed);//tänk på hur många layers det finns
            //Debug.Log(speed);
        }
        else  //if (Input.GetKey("s"))
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
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
            else if (animator.GetCurrentAnimatorStateInfo(1).IsName("Fire"))
            {
                if (speed < 1)
                    speed = speed + (chargeSpeed);

                animator.SetLayerWeight(1, speed);

                fiered = true;

            }


            //if (speed > 0)
             //   speed = speed - chargeSpeed;

            //animator.SetLayerWeight(1, speed);
            //Debug.Log(speed);
        }
    }


}


