using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Networking;

public class AnimationTest : NetworkBehaviour {

    public Animator animator;
    public float chargeSpeed = 0.01f;
    public float speed = 0;
    public float chargeStartWeight = 0.5f;
    //public MouseLook cameraRot;

    //public GameObject parentScript;
    public RigidbodyFirstPersonController controller;

    public CharacterFade test;

    public Transform spine;
    public Transform root;

    public float rotationSpeed = 100f;

    public bool fiered = false;

    public Transform characterRotation;



    float spineY;
    float spineZ;
    float characterY;
    float rotationZ;
    float rotationY;
    public float characterYStart;

    float _lastVelocity;

    // Use this for initialization
    void Start() {

        if (!isLocalPlayer)
            return;

        //cameraRot = GetComponent<MouseLook>();
        controller = GetComponent<RigidbodyFirstPersonController>();

        animator.SetFloat("Velocity", 0);
        animator.SetBool("Fire", false);
        characterRotation = transform.GetChild(1);
        //characterYStart = characterRotation.rotation.y;
        Debug.Log(characterYStart);
    }

    
    // Update is called once per frame
    void Update() {

        if (isLocalPlayer)
        {

            ChangeStance();

            if (!(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")))
            {
                animator.SetFloat("Velocity", 0);
                animator.speed = 1.0f;

                //spineY = 0;
                //spineY = -spineY;

            }

            else if (controller.Dashing)
            {
                //spineY = 0;
                rotationY = 0;

                animator.SetFloat("Velocity", 1);
                animator.speed = 5.0f;
            }

            /*else
            {
                animator.SetFloat("Velocity", 0);
                animator.speed = 1.0f;
                //animator.SetBool("s", false);
            }*/

            /*if(Input.GetKey("w")&& Input.GetKey(KeyCode.LeftShift))
            {
                animator.speed = 5.0f;
            }
            else
            {
                animator.speed = 1.0f;
            }*/

            if (controller.Jumping)
            {
                animator.SetBool("Jump", true);
                animator.SetBool("Land", false);
            }

            else if (controller.Grounded)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Land", true);
            }

            if (Input.GetKeyDown("k"))
            {
                animator.SetBool("Death", true);
                spineZ = 0;
                spineY = 0;
            }

            UpdateSpineRotation();
        }
    }

    void LateUpdate()
    {
        if (isLocalPlayer)
        {

            float rootAngel = 0;
            if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
            {

                //spineY = 0;

                //rotationY = Mathf.Lerp(rotationY, 0, Time.deltaTime*2.0f);

                animator.SetFloat("Velocity", 1);
                /*if (Input.GetKey("w"))
                {

                }*/

                if (Input.GetKey("a") && Input.GetKey("w"))
                {
                    rootAngel = 45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 45, root.eulerAngles.z);
                }
                else if (Input.GetKey("a") && Input.GetKey("s"))
                {
                    animator.SetFloat("Velocity", -1);
                    rootAngel = -45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 45, root.eulerAngles.z);
                }
                else if (Input.GetKey("s") && Input.GetKey("d"))
                {
                    animator.SetFloat("Velocity", -1);
                    rootAngel = 45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 45, root.eulerAngles.z);
                }
                else if (Input.GetKey("d") && Input.GetKey("w"))
                {
                    rootAngel = -45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 45, root.eulerAngles.z);
                }

                else if (Input.GetKey("a"))
                {
                    rootAngel = 90;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 90, root.eulerAngles.z);
                }

                else if (Input.GetKey("s"))
                {
                    //rootAngel = 180;

                    animator.SetFloat("Velocity", -1);
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y, root.eulerAngles.z);
                }

                else if (Input.GetKey("d"))
                {
                    rootAngel = -90;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 90, root.eulerAngles.z);
                }

                rotationY = 0;
            }
            if (Input.GetKey(KeyCode.Mouse0) || animator.GetCurrentAnimatorStateInfo(1).IsName("Fire") || fiered == false)
            {
                spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + rootAngel, spine.eulerAngles.z + spineZ);
            }


            root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - spineY, root.eulerAngles.z);
            spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + spineY, spine.eulerAngles.z + spineZ);

            
            CmdUpdateAnimator(animator.GetFloat("Velocity"));

            //parentScript.GetComponent<RigidbodyFirstPersonController>().cam.transform;
        } else
        {
            animator.SetFloat("Velocity", _lastVelocity);
        }
    }

    [Command]
    void CmdUpdateAnimator(float velocity)
    {
        RpcUpdateAnimator(velocity);
    }

    [ClientRpc]
    void RpcUpdateAnimator(float velocity)
    {
        _lastVelocity = velocity;
    }

    /* Om vapnet laddas så ska hela överkroppen roteras runt x-axeln mot siktet. 
       Om vapnet inte laddas ska bara huvudet roteras, tills man kollar tillräckligt långt åt sidan, då ska överkroppen roteras runt z-axeln. Kollar man ännu längre ska hela kroppen vändas mot siktet.
    */
    private void UpdateSpineRotation(){

       /* if (Input.GetKey("up"))
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
            //spineY = spineY + Time.deltaTime * rotationSpeed;
            if (Input.GetKey("w"))
            {
                rotationY += Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
                rotationY = Mathf.Clamp(rotationY, -45, 45);
                spineY = rotationY;
            }
            rotationZ += Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed;
            rotationZ = Mathf.Clamp(rotationZ, -45, 45);
            spineZ = rotationZ;
            
            

            Debug.Log(rotationY + ", " + rotationZ);    
        }
        */

        rotationY += Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
        rotationY = Mathf.Clamp(rotationY, -45, 45);
        spineY = rotationY;
        rotationZ += Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed;
        rotationZ = Mathf.Clamp(rotationZ, -45, 45);
        spineZ = rotationZ;


    }
   
    private void UpdateMovemtRotation()
    {
        

        if (Input.GetKey("a"))
        {
            characterY = characterYStart - 90;
        }
        else if (Input.GetKey("d"))
        {
            characterY = characterYStart + 90;
        }
        else
        {
            characterY = characterYStart;
        }
        
        characterRotation.rotation = Quaternion.Euler(0, characterY, 0);
    }


    private void ChangeStance()
    {
        if (speed > 0)
        {
            //UppdateMovemtRotation();
        }
        else
        {
            //characterRotation.transform.rotation = Quaternion.Euler(0, characterY, 0);
        } 

        

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


