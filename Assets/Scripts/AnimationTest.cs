using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

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

    public bool fired = false;

    public Transform characterRotation;

    float spineY;
    float spineZ;
    float characterY;
    float rotationZ;
    float rotationY;
    float rootAngle;

    public float characterYStart;

    float _lastVelocity;
    bool _lastJump;
    bool _lastLand;
    bool _lastDeath;
    bool _lastFire;
    float _lastWeight;
    Quaternion _lastSpineRot;
    Quaternion _lastRootRot;

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

        RigidbodyFirstPersonController.OnStartJump += Jump;
        RigidbodyFirstPersonController.OnDeath += Death;
        RigidbodyFirstPersonController.OnRespawn += Respawn;
    }

    void Jump()
    {
        animator.SetBool("Jump", true);
        //animator.SetBool("Land", false);
    }

    void Death()
    {
        animator.SetTrigger("Death");
        spineZ = 0;
        spineY = 0;
        rootAngle = 0;
        CmdUpdateDeath(animator.GetBool("Death"));
    }
   
    void Respawn()
    {
        animator.SetTrigger("Respawn");
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
            } else if (!controller.Dashing)
            {
                animator.speed = 1;
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
                //
                //animator.SetBool("Jump", true);
                //animator.SetBool("Land", false);
            }

            else if (controller.Grounded)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Land", true);
            }

            //if (Input.GetKeyDown("k"))
            //{
            //    animator.SetBool("Death", true);
            //    spineZ = 0;
            //    spineY = 0;
            //}

            UpdateSpineRotation();
        } else
        {
            
            //spine.rotation = Quaternion.RotateTowards(spine.rotation, _lastSpineRot, Time.deltaTime);
            //spine.rotation = Quaternion.Lerp(spine.rotation.normalized, _lastSpineRot.normalized, 1);
            //spine.rotation = _lastSpineRot;
            //root.rotation = Quaternion.RotateTowards(root.rotation, _lastRootRot, Time.deltaTime);
            //root.rotation = Quaternion.Lerp(root.rotation.normalized, _lastRootRot.normalized, 1);
            //root.rotation = _lastRootRot;

        }
    }

    void LateUpdate()
    {
        if (isLocalPlayer)
        {

            if (controller.Dead)
                return;

            rootAngle = 0;
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
                    rootAngle = 45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 45, root.eulerAngles.z);
                }
                else if (Input.GetKey("a") && Input.GetKey("s"))
                {
                    animator.SetFloat("Velocity", -1);
                    rootAngle = -45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 45, root.eulerAngles.z);
                }
                else if (Input.GetKey("s") && Input.GetKey("d"))
                {
                    animator.SetFloat("Velocity", -1);
                    rootAngle = 45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 45, root.eulerAngles.z);
                }
                else if (Input.GetKey("d") && Input.GetKey("w"))
                {
                    rootAngle = -45;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 45, root.eulerAngles.z);
                }

                else if (Input.GetKey("a"))
                {
                    rootAngle = 90;
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
                    rootAngle = -90;
                    root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 90, root.eulerAngles.z);
                }

                rotationY = 0;
            }
            if (Input.GetKey(KeyCode.Mouse0) || animator.GetCurrentAnimatorStateInfo(1).IsName("Fire") || fired == false)
            {
                spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + rootAngle, spine.eulerAngles.z);
            }


            root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - spineY, root.eulerAngles.z);
            spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + spineY, spine.eulerAngles.z + spineZ);

            if (_lastVelocity != animator.GetFloat("Velocity"))
                CmdUpdateVelocity(animator.GetFloat("Velocity"));

            if (_lastDeath != animator.GetBool("Death"))
                CmdUpdateDeath(animator.GetBool("Death"));

            if (_lastJump != animator.GetBool("Jump"))
                CmdUpdateJump(animator.GetBool("Jump"));

            if (_lastLand != animator.GetBool("Land"))
                CmdUpdateLand(animator.GetBool("Land"));

            if (_lastFire != animator.GetBool("Fire"))
                CmdUpdateFire(animator.GetBool("Fire"));

            if (Quaternion.Angle(_lastRootRot, root.rotation) > ServerStatsManager.instance.maxRotationUpdateLimit)
            {
                CmdUpdateRootRot(root.rotation);
                _lastRootRot = root.rotation;
            }

            if (Quaternion.Angle(_lastSpineRot, spine.rotation) > ServerStatsManager.instance.maxRotationUpdateLimit)
            {
                CmdUpdateSpineRot(spine.rotation);
                _lastSpineRot = spine.rotation;
            }

            if (_lastWeight != animator.GetLayerWeight(1))
            {
                CmdUpdateWeight(animator.GetLayerWeight(1));
            }

            //parentScript.GetComponent<RigidbodyFirstPersonController>().cam.transform;
        } else
        {
            animator.SetFloat("Velocity", _lastVelocity);
            animator.SetBool("Death", _lastDeath);
            animator.SetBool("Jump", _lastJump);
            animator.SetBool("Land", _lastLand);
            animator.SetBool("Fire", _lastFire);

            //print("Real Spine: " + spine.rotation.normalized.ToString() + " Last Spine: " + _lastSpineRot.normalized.ToString());

            //root.rotation = Quaternion.Lerp(root.rotation, _lastRootRot, Time.deltaTime * 10f);
            //root.rotation = Quaternion.RotateTowards(root.rotation, _lastRootRot, Time.deltaTime * 30f);
            root.rotation = _lastRootRot;
            //spine.rotation = Quaternion.Lerp(spine.rotation, _lastSpineRot, Time.deltaTime * 10f);
            //spine.rotation = Quaternion.RotateTowards(spine.rotation, _lastSpineRot, Time.deltaTime * 30f);
            spine.rotation = _lastSpineRot;

            animator.SetLayerWeight(1, _lastWeight); //GÖR OM TILL TRIGGER ON CHARGE + ON SHOOT
        }
    }

    #region serverStuff
    [Command]
    void CmdUpdateVelocity(float velocity)
    {
        RpcUpdateVelocity(velocity);
    }

    [Command]
    void CmdUpdateDeath(bool death)
    {
        RpcUpdateDeath(death);
    }

    [Command]
    void CmdUpdateJump(bool jump)
    {
        RpcUpdateJump(jump);
    }

    [Command]
    void CmdUpdateLand(bool land)
    {
        RpcUpdateLand(land);
    }

    [Command]
    void CmdUpdateFire(bool fire)
    {
        RpcUpdateFire(fire);
    }

    [Command]
    void CmdUpdateSpineRot(Quaternion rot)
    {
        RpcUpdateSpineRot(rot);
    }

    [Command]
    void CmdUpdateRootRot(Quaternion rot)
    {
        RpcUpdateRootRot(rot);
    }

    [Command]
    void CmdUpdateWeight(float weight)
    {
        RpcUpdateWeight(weight);
    }

    [ClientRpc]
    void RpcUpdateVelocity(float velocity)
    {
        _lastVelocity = velocity;
    }

    [ClientRpc]
    void RpcUpdateDeath(bool death)
    {
        _lastDeath = death;
    }

    [ClientRpc]
    void RpcUpdateJump(bool jump)
    {
        _lastJump = jump;
    }

    [ClientRpc]
    void RpcUpdateLand(bool land)
    {
        _lastLand = land;
    }

    [ClientRpc]
    void RpcUpdateFire(bool fire)
    {
        _lastFire = fire;
    }

    [ClientRpc]
    void RpcUpdateSpineRot(Quaternion rot)
    {
        _lastSpineRot = rot;
    }

    [ClientRpc]
    void RpcUpdateRootRot(Quaternion rot)
    {
        _lastRootRot = rot;
    }

    [ClientRpc]
    void RpcUpdateWeight(float weight)
    {
        _lastWeight = weight;
    }

    #endregion

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
        rotationZ = Mathf.Clamp(rotationZ, -90, 45);
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
            fired = false;

            
            if (speed<1)
                speed = speed + chargeSpeed;

            animator.SetLayerWeight(1, speed);//tänk på hur många layers det finns
            //Debug.Log(speed);
        }
        else  //if (Input.GetKey("s"))
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
                animator.SetBool("Fire", true);
               


            if (fired && animator.GetCurrentAnimatorStateInfo(1).IsName("Charge"))
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

                fired = true;

            }


            //if (speed > 0)
             //   speed = speed - chargeSpeed;

            //animator.SetLayerWeight(1, speed);
            //Debug.Log(speed);
        }
    }
}


