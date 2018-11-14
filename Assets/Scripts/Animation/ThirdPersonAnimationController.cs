using UnityEngine;
using UnityEngine.Networking;

public class ThirdPersonAnimationController : NetworkBehaviour {


    public float chargeSpeed = 0.01f;
    public float speed = 0f;
    public float chargeStartWeight = 0.5f;
    public float rotationSpeed = 35f;
    public float characterYStart = 90f;

    public Transform spine;
    public Transform root;

    private PlayerController controller;
    public Animator thirdPersonAnimator;
    public Animator firstPersonAnimator;
    

    private Transform model;


    public bool fired = false;

    float spineY;
    float spineZ;
    float characterY;
    float rotationZ;
    float rotationY;
    float rootAngle;

    float _lastVelocity;
    bool _lastJump;
    bool _lastLand;
    bool _lastDeath;
    bool _lastFire;
    bool _lastRespawn;
    float _lastWeight;
    Quaternion _lastSpineRot;
    Quaternion _lastRootRot;
    private bool charging = false;

    void Start() {

        rotationSpeed = 70f;

        //if (!isLocalPlayer)
        //return;

        controller = GetComponent<PlayerController>();
        model = GetComponentInChildren<SkinnedMeshRenderer>().transform;

        thirdPersonAnimator.SetFloat("Velocity", 0);
        firstPersonAnimator.SetFloat("Velocity", 0);
        thirdPersonAnimator.SetBool("Fire", false);
        firstPersonAnimator.SetBool("Fire", false);
        //characterYStart = characterRotation.rotation.y;

        //GetComponent<RigidbodyFirstPersonController>().OnStartJump += Jump;
        //GetComponent<RigidbodyFirstPersonController>().OnDeath += CmdTriggerDeath;
        //GetComponent<RigidbodyFirstPersonController>().OnRespawn += CmdTriggerRespawn;
    }

    private void OnEnable()
    {
        GetComponent<PlayerController>().OnDeath += Death;
        GetComponent<PlayerController>().OnRespawn += Respawn;
    }

    void Jump()
    {
        thirdPersonAnimator.SetBool("Jump", true);
        firstPersonAnimator.SetBool("Jump", true);
        thirdPersonAnimator.SetBool("Land", false);
        firstPersonAnimator.SetBool("Land", false);
    }

    void Death()
    {
        print(GetComponent<PlayerID>().playerID.ToString() + " HAS DIED");
        thirdPersonAnimator.SetBool("Respawn", false);
        thirdPersonAnimator.SetBool("Death", true);
        spineZ = 0;
        spineY = 0;
        rootAngle = 0;

        root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - spineY, root.eulerAngles.z);
        spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + spineY, spine.eulerAngles.z + spineZ);

        //if (isLocalPlayer)
        //    CmdTriggerDeath();
    }
    
    void Respawn()
    {
        thirdPersonAnimator.SetBool("Death", false);
        thirdPersonAnimator.SetBool("Respawn", true);
        print(GetComponent<PlayerID>().playerID.ToString() + " HAS RESPAWNED");
    }

    void Update() {

        if (isLocalPlayer)
        {

            ChangeStance();

            if (!(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")))
            {
                thirdPersonAnimator.SetFloat("Velocity", 0);
                firstPersonAnimator.SetFloat("Velocity", 0);
                thirdPersonAnimator.speed = 1.0f;

                //spineY = 0;
                //spineY = -spineY;

            }

            if (controller.Dashing)
            {
                //spineY = 0;
                rotationY = 0;

                thirdPersonAnimator.SetFloat("Velocity", 1);
                thirdPersonAnimator.speed = 5.0f;
                firstPersonAnimator.SetBool("Dash", true);
            } else if (!controller.Dashing)
            {
                firstPersonAnimator.SetBool("Dash", false);
                thirdPersonAnimator.speed = 1;
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
                thirdPersonAnimator.SetBool("Jump", true);
                thirdPersonAnimator.SetBool("Land", false);
                firstPersonAnimator.SetBool("Jump", true);
                firstPersonAnimator.SetBool("Land", false);
            }

            else if (controller.Grounded)
            {

                thirdPersonAnimator.SetBool("Jump", false);
                thirdPersonAnimator.SetBool("Land", true);
                firstPersonAnimator.SetBool("Jump", false);
                firstPersonAnimator.SetBool("Land", true);
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

    float lerpValue=0;
    float spineLerpValue = 0;

    void LateUpdate()
    {
        if (isLocalPlayer)
        {

            if (!controller.Dead)
            {
                
                rootAngle = 0;
                if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
                {

                    //spineY = 0;

                    //rotationY = Mathf.Lerp(rotationY, 0, Time.deltaTime*2.0f);

                    thirdPersonAnimator.SetFloat("Velocity", 1);
                    firstPersonAnimator.SetFloat("Velocity", 1);
                    /*if (Input.GetKey("w"))
                    {

                    }*/
                   
                    if (Input.GetKey("w"))
                    {
                        rootAngle = 0;
                        //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 90, root.eulerAngles.z);
                        if (Input.GetKey("a") && Input.GetKey("w"))
                        {
                            rootAngle = 45;
                            //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 45, root.eulerAngles.z);
                        }
                        else if (Input.GetKey("d") && Input.GetKey("w"))
                        {
                            rootAngle = -45;
                            //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 45, root.eulerAngles.z);
                        }
                    }

                    

                    else if (Input.GetKey("s"))
                    {
                        //rootAngel = 180;

                        thirdPersonAnimator.SetFloat("Velocity", -1);
                        firstPersonAnimator.SetFloat("Velocity", -1);
                        rootAngle = 0;

                        if (Input.GetKey("a") && Input.GetKey("s"))
                        {
                            
                            rootAngle = -45;
                            //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 45, root.eulerAngles.z);
                        }
                        else if (Input.GetKey("s") && Input.GetKey("d"))
                        {
                            
                            rootAngle = 45;
                            //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 45, root.eulerAngles.z);
                        }

                        //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y, root.eulerAngles.z);
                    }

                    else if (Input.GetKey("a"))
                    {
                        rootAngle = 90;
                        //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - 90, root.eulerAngles.z);
                    }
                    else if (Input.GetKey("d"))
                    {
                        rootAngle = -90;
                        //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y + 90, root.eulerAngles.z);
                    }


                    /*Vector3 startVector = new Vector3(
                        Mathf.LerpAngle(root.eulerAngles.x, root.eulerAngles.x, Time.deltaTime),
                        Mathf.LerpAngle(root.eulerAngles.y, root.eulerAngles.y + (rootAngle * -1), Time.deltaTime),
                        Mathf.LerpAngle(root.eulerAngles.z, root.eulerAngles.z, Time.deltaTime));*/

                   if(rootAngle <0 && lerpValue>rootAngle)
                        lerpValue=lerpValue-5;
                   else if (rootAngle > 0 && lerpValue < rootAngle)
                        lerpValue=lerpValue+5;
                   

                    rotationY = 0;


                }
                if (rootAngle == 0)
                {
                    if (lerpValue > 0)
                        lerpValue = lerpValue - 5;
                    else if (lerpValue < 0)
                        lerpValue = lerpValue + 5;
                }

                /*else
                {
                    if (lerpValue > 0)
                        lerpValue = lerpValue - 5;
                    else if (lerpValue < 0 )
                        lerpValue = lerpValue + 5;
                    //Y = 0;
                }*/

                Vector3 lerpVector = new Vector3(root.eulerAngles.x, root.eulerAngles.y + (lerpValue * -1), root.eulerAngles.z);

                root.eulerAngles = lerpVector; //vet inte hur man lerpar

                if (Input.GetKey(KeyCode.Mouse0) || thirdPersonAnimator.GetCurrentAnimatorStateInfo(1).IsName("Fire") || fired == false)
                {
                    if (rootAngle < 0 && spineLerpValue > rootAngle)
                        spineLerpValue = spineLerpValue - 5;
                    else if (rootAngle > 0 && spineLerpValue < rootAngle)
                        spineLerpValue = spineLerpValue + 5;

                    //spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + spineLerpValue, spine.eulerAngles.z);
                }
                //else
                if (rootAngle == 0)
                {
                    if (spineLerpValue > 0)
                        spineLerpValue = spineLerpValue - 5;
                    else if (spineLerpValue < 0)
                        spineLerpValue = spineLerpValue + 5;
                }



                //root.eulerAngles = new Vector3(root.eulerAngles.x, root.eulerAngles.y - spineY, root.eulerAngles.z);//Motverkar överkropps rotation, får modellen att röra rig skumt i idel
                //spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y + spineY, spine.eulerAngles.z + spineZ); //roterar överkropp delvis
                //if(rotationZ <90&& rotationZ > -45)
                    spine.eulerAngles = new Vector3(spine.eulerAngles.x, spine.eulerAngles.y+ spineLerpValue, spine.eulerAngles.z + spineZ); //kan bara kolla upp och ner

            }

            if (_lastVelocity != thirdPersonAnimator.GetFloat("Velocity"))
                CmdUpdateVelocity(thirdPersonAnimator.GetFloat("Velocity"));

            if (_lastDeath != thirdPersonAnimator.GetBool("Death"))
                //CmdUpdateDeath(animator.GetBool("Death"));

            if (_lastRespawn != thirdPersonAnimator.GetBool("Respawn"))
                //CmdUpdateRespawn(animator.GetBool("Respawn"));

            if (_lastJump != thirdPersonAnimator.GetBool("Jump"))
                CmdUpdateJump(thirdPersonAnimator.GetBool("Jump"));

            if (_lastLand != thirdPersonAnimator.GetBool("Land"))
                CmdUpdateLand(thirdPersonAnimator.GetBool("Land"));

            if (_lastFire != thirdPersonAnimator.GetBool("Fire"))
                CmdUpdateFire(thirdPersonAnimator.GetBool("Fire"));

            if (Quaternion.Angle(_lastRootRot, root.rotation) > 5f)
            {
                CmdUpdateRootRot(root.rotation);
                _lastRootRot = root.rotation;
            }

            if (Quaternion.Angle(_lastSpineRot, spine.rotation) > 5f)
            {
                CmdUpdateSpineRot(spine.rotation);
                _lastSpineRot = spine.rotation;
            }

            if (_lastWeight != thirdPersonAnimator.GetLayerWeight(1))
            {
                CmdUpdateWeight(thirdPersonAnimator.GetLayerWeight(1));
            }

            //parentScript.GetComponent<RigidbodyFirstPersonController>().cam.transform;
        } else
        {
            thirdPersonAnimator.SetFloat("Velocity", _lastVelocity);
            //animator.SetBool("Death", _lastDeath);
            //animator.SetBool("Respawn", _lastRespawn);
            thirdPersonAnimator.SetBool("Jump", _lastJump);
            thirdPersonAnimator.SetBool("Land", _lastLand);
            thirdPersonAnimator.SetBool("Fire", _lastFire);

            //print("Real Spine: " + spine.rotation.normalized.ToString() + " Last Spine: " + _lastSpineRot.normalized.ToString());

            //root.rotation = Quaternion.Lerp(root.rotation, _lastRootRot, Time.deltaTime * 10f);
            //root.rotation = Quaternion.RotateTowards(root.rotation, _lastRootRot, Time.deltaTime * 30f);
            root.rotation = _lastRootRot;
            //spine.rotation = Quaternion.Lerp(spine.rotation, _lastSpineRot, Time.deltaTime * 10f);
            //spine.rotation = Quaternion.RotateTowards(spine.rotation, _lastSpineRot, Time.deltaTime * 30f);
            spine.rotation = _lastSpineRot;

            thirdPersonAnimator.SetLayerWeight(1, _lastWeight); //GÖR OM TILL TRIGGER ON CHARGE + ON SHOOT
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
    void CmdTriggerDeath()
    {
        if (!isLocalPlayer)
            RpcTriggerDeath();
    }

    [Command]
    void CmdUpdateRespawn(bool respawn)
    {
        RpcUpdateRespawn(respawn);
    }

    [Command]
    void CmdTriggerRespawn()
    {
        RpcTriggerRespawn();
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
    void RpcTriggerDeath()
    {
        Death();
    }

    [ClientRpc]
    void RpcUpdateRespawn(bool respawn)
    {
        _lastRespawn = respawn;
    }

    [ClientRpc]
    void RpcTriggerRespawn()
    {
        Respawn();
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

    float realRotationY;
    float realRotationZ;

    private void UpdateSpineRotation(){

        

        

        rotationY += Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
        rotationY = Mathf.Clamp(rotationY, -45, 45);
        spineY = rotationY;

        

        realRotationZ  += Input.GetAxis("Mouse Y");
        
        realRotationZ = Mathf.Clamp(realRotationZ, controller.mouseLook.MinimumX, controller.mouseLook.MaximumX);

        //Debug.Log(realRotationZ+" "+( Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed));  //rotationSpeed måste ändras i preefab

        if (realRotationZ<45&& realRotationZ>-45) {
            rotationZ = realRotationZ;
            rotationZ = Mathf.Clamp(rotationZ, -45, 45);
            spineZ = rotationZ;
        }

    }
   
    private void UpdateMovemtRotation() //används denna?
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
        
        model.rotation = Quaternion.Euler(0, characterY, 0);
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

        //if (Input.GetKeyDown(KeyCode.Mouse1) && controller.canShoot && controller._shootCooldownDone)
        //{
        //    firstPersonAnimator.SetBool("Fire", false);
        //    firstPersonAnimator.SetBool("Charge", false);
        //    thirdPersonAnimator.SetBool("Fire", false);
        //}

        /*if (Input.GetKeyDown(KeyCode.Mouse0) && controller.canShoot && controller._shootCooldownDone) {//borde göra så att övergången till chargen blir långamare ju mer tid som har gått
            speed = chargeStartWeight;
        }*/

        if (Input.GetKey(KeyCode.Mouse0) && controller.canShoot && controller._shootCooldownDone)
        {
            //thirdPersonAnimator.SetBool("Fire", false);
            //firstPersonAnimator.SetBool("Fire", false);
            //firstPersonAnimator.SetBool("Charge", true);
            //fired = false;

            
            //if (speed<1)
            //    speed = speed + chargeSpeed;

            //thirdPersonAnimator.SetLayerWeight(1, speed);//tänk på hur många layers det finns
            //Debug.Log(speed);
        }
        else  //if (Input.GetKey("s"))
        {
            //if (Input.GetKeyUp(KeyCode.Mouse0))
            //{
            //    thirdPersonAnimator.SetBool("Fire", true);
            //    firstPersonAnimator.SetBool("Fire", true);
            //    firstPersonAnimator.SetBool("Charge", false);
            //}




            if (fired && thirdPersonAnimator.GetCurrentAnimatorStateInfo(1).IsName("Charge"))
            {

                //if (speed > 0)
                //    speed = speed - (chargeSpeed * 5);

                ////thirdPersonAnimator.SetBool("Fire", false);
                ////firstPersonAnimator.SetBool("Fire", false);
                ////animator.SetBool("Fire", false);
                ////speed = 0;

                //thirdPersonAnimator.SetLayerWeight(1, speed);


                
                //if (speed > 0)
                //    speed = speed - (chargeSpeed*5);
                

                //thirdPersonAnimator.SetLayerWeight(1, speed);
                
                //fiered = false;
            }
            else if (thirdPersonAnimator.GetCurrentAnimatorStateInfo(1).IsName("Fire"))
            {
                //if (speed < 1)
                //    speed = speed + (chargeSpeed);

                //thirdPersonAnimator.SetLayerWeight(1, speed);

                fired = true;

            }



            //if (speed > 0)
            //   speed = speed - chargeSpeed;

            //thirdPersonAnimator.SetLayerWeight(1, speed);
            //Debug.Log(speed);
        }

        if (fired && thirdPersonAnimator.GetCurrentAnimatorStateInfo(1).IsName("Charge")) {
            if (speed > 0)
                speed = speed - chargeSpeed * 2;

            thirdPersonAnimator.SetLayerWeight(1, speed);
        }

        else if (charging || thirdPersonAnimator.GetCurrentAnimatorStateInfo(1).IsName("Fire")) {
            if (speed < 1)
                speed = speed + (chargeSpeed);

            thirdPersonAnimator.SetLayerWeight(1, speed);
        }
        else if (!charging && fired) {
            if (speed > 0)
                speed = speed - chargeSpeed*2;
            
            thirdPersonAnimator.SetLayerWeight(1, speed);
        }
    }

    public void StartCharge()
    {
        thirdPersonAnimator.SetBool("Fire", false);
        firstPersonAnimator.SetBool("Fire", false);
        firstPersonAnimator.SetBool("Charge", true);
        fired = false;
        charging = true;
    }

    public void Shoot()
    {
        thirdPersonAnimator.SetBool("Fire", true);
        firstPersonAnimator.SetBool("Fire", true);
        firstPersonAnimator.SetBool("Charge", false);
        charging = false;
    }

    public void CancelCharge()
    {
        firstPersonAnimator.SetBool("Fire", false);
        firstPersonAnimator.SetBool("Charge", false);
        thirdPersonAnimator.SetBool("Fire", false);
        charging = false;
        fired = true;
    }
}


