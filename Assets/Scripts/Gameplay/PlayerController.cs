using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(CapsuleCollider))]
[NetworkSettings(channel = 0, sendInterval = 0.1f)] //
public class PlayerController : NetworkBehaviour
{
    [Serializable]
    public class MovementSettings
    {
        public float forwardSpeed = 8.0f;   // Speed when walking forward
        public float backwardSpeed = 4.0f;  // Speed when walking backwards
        public float strafeSpeed = 4.0f;    // Speed when walking sideways

        public float jumpForce = 30f;
        public float groundedDrag = 5f;
        public float jumpDrag = 0f;
        public float dashDrag = 0f;
        public float slowDownLimit = 0.75f;
        public AnimationCurve slopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        [HideInInspector] public float currentTargetSpeed = 8f;

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero) return;
            if (input.x > 0 || input.x < 0)
            {
                //strafe
                currentTargetSpeed = strafeSpeed;
            }
            if (input.y < 0)
            {
                //backwards
                currentTargetSpeed = backwardSpeed;
            }
            if (input.y > 0)
            {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                currentTargetSpeed = forwardSpeed;
            }
        }
    }

    [Serializable]
    public class AdvancedSettings
    {
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float stickToGroundHelperDistance = 0.5f; // stops the character
        public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
        public float speedUpRate = 10f;
        public bool airControl; // can the user control the direction that is being moved in the air
        public float airTimeLimit = 0.2f;
        public float airSpeedUpRate = 5f;
        [Tooltip("set it to 0.1 or more if you get stuck in wall")]
        public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
    }

    public Camera cam;
    public Camera deathCamera;
    public MovementSettings movementSettings = new MovementSettings();
    public MouseLook mouseLook = new MouseLook();
    public AdvancedSettings advancedSettings = new AdvancedSettings();

    public Component[] componentsToDisable;

    [SyncVar]
    public float movementUpdateRate;

    [Header("Shooting")]
    public Transform beamOrigin;
    public GameObject beam;
    public GameObject thirdPersonChargeEffect;
    public GameObject firstPersonChargeEffect;
    public float beamDistanceMultiplier = 1f;
    public float beamMinDistance;
    public float beamMaxDistance;
    public float beamDistance;
    public float sphereCastWidth = 0.1f;
    public float shootCooldown;
    public float beamSlowMultiplier = 0.08f;

    [Header("Dash")]
    public float dashDuration;
    public float dashForce;
    public float dashCooldown;

    private Rigidbody m_RigidBody;
    private CapsuleCollider m_Capsule;
    private float m_YRotation;
    private Vector3 m_GroundContactNormal;
    private bool hasJumped, wasPreviouslyGrounded, isJumping, isGrounded, isDashing, isDead, isCharging;
    private Coroutine thirdPersonCharge, chargeSound;
    private float airTime;

    private Vector3 _lastPosition;
    private Vector3 _mylastPosition;
    private Quaternion _lastRotation;
    [HideInInspector]
    public bool _shootCooldownDone = true;
    [HideInInspector]
    public bool chargingShot = false;

    public Team myTeam;
    public int myTeamID;
    public bool canMove = true;
    public bool canShoot = true;
    public bool canDash = true;

    public delegate void ControllerEvent();
    public event ControllerEvent OnStartJump;
    public event ControllerEvent OnDeath;
    public event ControllerEvent OnRespawn;
    public event ControllerEvent OnShoot;
    //public event ControllerIDEvent OnDash;

    [Header("Asset")]
    public TeamAsset teamLightAsset;
    public TeamAsset teamShadowAsset;
    public TeamAsset myAsset;

    private float forwardRate;
    private float strafeRate;

    public AudioSource runSource;
    public AudioSource chargeSource;

    public ThirdPersonAnimationController animationController;

    public PostProcessProfile postProcess;
    private ChromaticAberration chrome;

    public enum Team
    {
        Light = 1,
        Shadow
    };

    public Vector3 Velocity
    {
        get { return m_RigidBody.velocity; }
    }

    public bool Grounded
    {
        get { return isGrounded; }
    }

    public bool Jumping
    {
        get { return isJumping; }
    }

    public bool Dashing
    {
        get
        {
            return isDashing;
        }
    }

    public bool Dead
    {
        get { return isDead; }
    }

    private void Start()
    {
        Setup();
    }

    //public override void OnStartLocalPlayer()
    //{
    //    SetupSelf();
    //}


    public void Setup() //OPPPS
    {
        AssignTeam();
        animationController = GetComponent<ThirdPersonAnimationController>();
        if (!isLocalPlayer)
        {
            foreach (Behaviour component in componentsToDisable)
            {
                component.enabled = false;
            }
            GetComponent<MaterialSwap>().firstPersonModel.enabled = false;
            firstPersonChargeEffect.SetActive(false);
        }
        else
        {
            transform.gameObject.layer = 2;
            //SoundManager.instance.AddSoundOnStart(this);
            if(SoundManager.instance)
                SoundManager.instance.SetPlayerOrigin(gameObject);
            GetComponent<MaterialSwap>().thirdPersonModel.gameObject.layer = 9;
            GetComponent<MaterialSwap>().thirdPersonMask.gameObject.layer = 9;
            thirdPersonChargeEffect.SetActive(false);
            GetComponent<DecoySpawn>().targetTransparency = GetComponent<MaterialSwap>().firstPersonTransperancy;
            postProcess.TryGetSettings<ChromaticAberration>(out chrome);
        }
        m_RigidBody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        mouseLook.Init(transform, cam.transform);
        //OnDash += PlayDashSound;
        //OnDash += CmdPlayDashSound;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {

            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !hasJumped && !isDashing)
            {
                hasJumped = true;
                //CmdPlayJumpSound();
            }

            ShootCheck();

            //if (_mylastPosition != transform.position) //Ändra till 0.1 skillnad 
            //{
            CmdUpdatePosition(transform.position);
                //_mylastPosition = transform.position;
           // }

            if (_lastRotation != transform.rotation)
            {
                CmdUpdateRotation(transform.rotation);
                _lastRotation = transform.rotation;
            }
        }
        else
        {
            transform.rotation = _lastRotation;
        }
    }


    [Command]
    private void CmdPlayJumpSound()
    {
        RpcPlayJumpSound();
    }

    [ClientRpc]
    private void RpcPlayJumpSound()
    {
        if (!isLocalPlayer)
            SoundManager.instance.PlayJumpSound();
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (!isDashing)
            {
                GroundCheck();
                Movement();
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && canMove && !chargingShot)
            {
                //StartCoroutine(InitiateDash());
                StartCoroutine(InitiateDash2());
            }
            else if(Input.GetKeyDown(KeyCode.LeftShift) && (!canDash || !canMove || chargingShot))
            {
                //Play local error sound
                Debug.Log("Error");
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * movementUpdateRate);
        }
    }

    private void Movement()
    {
       


        Vector2 input = GetRawInput();//GetInput();

        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || isGrounded) && canMove)
        {

            RunMan();

            //float speedUpRate = isGrounded ? advancedSettings.speedUpRate : advancedSettings.airSpeedUpRate;
            // always move along the camera forward as it is the direction that it being aimed at
            //Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            forwardRate = Mathf.Lerp(forwardRate, input.y, Time.deltaTime * advancedSettings.speedUpRate);
            strafeRate = Mathf.Lerp(strafeRate, input.x, Time.deltaTime * advancedSettings.speedUpRate);


            Vector3 desiredMove = cam.transform.forward * forwardRate + cam.transform.right * strafeRate;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
            if (!isGrounded) desiredMove *= advancedSettings.airSpeedUpRate;
            desiredMove.x = desiredMove.x * movementSettings.currentTargetSpeed;
            desiredMove.z = desiredMove.z * movementSettings.currentTargetSpeed;
            //desiredMove.y = desiredMove.y * movementSettings.currentTargetSpeed;
            if (m_RigidBody.velocity.sqrMagnitude <
                (movementSettings.currentTargetSpeed * movementSettings.currentTargetSpeed))
            {
                m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
            }
        }

        if (isGrounded)
        {
            GetComponent<Rigidbody>().useGravity = false;
            m_RigidBody.drag = movementSettings.groundedDrag;

            if (hasJumped)
            {
                m_RigidBody.drag = movementSettings.jumpDrag;
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                m_RigidBody.AddForce(new Vector3(0f, movementSettings.jumpForce, 0f), ForceMode.Impulse);
                isJumping = true;

                //if (OnStartJump != null)
                    //OnStartJump();

            }

            //if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            //{
            //    m_RigidBody.velocity = 0;
            //}

            if (!isJumping && Mathf.Abs(input.x) < movementSettings.slowDownLimit && Mathf.Abs(input.y) < movementSettings.slowDownLimit)
            {

                m_RigidBody.velocity = Vector3.Lerp(m_RigidBody.velocity, Vector3.zero, Time.deltaTime * advancedSettings.slowDownRate);
                forwardRate = 0;
                strafeRate = 0;
                //if (m_RigidBody.velocity.magnitude < 1f)
                //{
                //    //m_RigidBody.velocity = Vector3.zero;
                //    m_RigidBody.velocity = Vector3.Lerp(m_RigidBody.velocity, Vector3.zero, Time.deltaTime * advancedSettings.slowDownRate);
                //}
                //else
                //{
                //    print("STOP RIGHT THERE CRIMINAL SCUM");
                //    //m_RigidBody.velocity *= 1 / (advancedSettings.slowDownRate);
                //    m_RigidBody.velocity = Vector3.zero;
                //}


            }
        }
        else
        {
            m_RigidBody.drag = 0f;
            if (wasPreviouslyGrounded && !isJumping)
            {
                //StickToGroundHelper();
            }
            GetComponent<Rigidbody>().useGravity = true;
        }
        hasJumped = false;
    }

    [Command]
    private void CmdUpdatePosition(Vector3 pos)
    {
        RpcUpdatePosition(pos);
    }

    [ClientRpc]
    private void RpcUpdatePosition(Vector3 pos)
    {
        _lastPosition = pos;
    }

    [Command]
    private void CmdUpdateRotation(Quaternion rot)
    {
        RpcUpdateRotation(rot);
    }

    [ClientRpc]
    private void RpcUpdateRotation(Quaternion rot)
    {
        _lastRotation = rot;
    }

    private void ShootCheck()
    {
        if (_shootCooldownDone && canShoot)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                chargingShot = true;
                animationController.StartCharge();
            }

            if (chargingShot)
            {
                ChargingShot();

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    ShootSphereCast();
                    animationController.Shoot();
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    CancelCharge();
                    animationController.CancelCharge();
                }
            }
        }
        else if(!_shootCooldownDone&&Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Play local error sound
            Debug.Log("Error");
        }
    }

    private void CancelCharge()
    {
        chargingShot = false;
        PersonalUI.instance.UpdateShootCharge(0, beamMaxDistance);
        beamDistance = 0;
        firstPersonChargeEffect.transform.localScale = Vector3.zero;
        CmdStopThirdPersonCharge();
    }

    private void AssignTeam()
    {
        if (isServer)
        {
            if (isLocalPlayer)
            {
                AssignTeamWhite();
            }
            else
            {
                AssignTeamBlack();
            }
        }

        else
        {
            if (isLocalPlayer)
            {
                AssignTeamBlack();

                //PersonalUI.instance.crosshair.sprite = UIChanges[0];


            }
            else
            {
                AssignTeamWhite();
            }
        }
        AssignColors();
    }

    private void AssignTeamWhite()
    {
        myTeam = Team.Light;
        myTeamID = 1;
        myAsset = teamLightAsset;

    }

    private void AssignTeamBlack()
    {
        myTeam = Team.Shadow;
        myTeamID = 2;
        myAsset = teamShadowAsset;
    }

    private void AssignColors()
    {
        ParticleSystem.ColorOverLifetimeModule col = GetComponent<MaterialSwap>().invisibleTrail.colorOverLifetime;
        col.color = myAsset.particleGradient;
        beam.GetComponent<LineRenderer>().colorGradient = myAsset.laserGradient;
        firstPersonChargeEffect.GetComponent<Renderer>().material.color = myAsset.bodyColor;
        thirdPersonChargeEffect.GetComponent<Renderer>().material.color = myAsset.bodyColor;

    }

    public void PlayDashSound(int playerID) {
        SoundManager.instance.PlayDashSound(playerID);
    }

    [Command]
    public void CmdPlayDashSound(int playerID) {
        RpcPlayDashSound(playerID);
    }

    [ClientRpc]
    public void RpcPlayDashSound(int playerID) {
        if (!isLocalPlayer) {
            PlayDashSound(playerID);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Objective") && isLocalPlayer && !Dead) {
            CmdCollectObjective(myTeamID, other.GetComponentInParent<NetworkIdentity>().netId);
        }
    }

    [Command]
    void CmdCollectObjective(int teamID, NetworkInstanceId objectiveID)
    {
        RpcCollectObjective(teamID, objectiveID);
    }

    [ClientRpc]
    void RpcCollectObjective(int teamID, NetworkInstanceId objectiveID)
    {
        GameObject objectiveSpawner = ClientScene.FindLocalObject(objectiveID);
        objectiveSpawner.GetComponent<ObjectiveSpawner>().CollectObjective(teamID);

    }

    private IEnumerator InitiateDash()
    {
        PlayDashSound(GetComponent<PlayerID>().playerID);
        CmdPlayDashSound(GetComponent<PlayerID>().playerID);
        Vector3 prevVelocity = new Vector3(
            m_RigidBody.velocity.normalized.x * movementSettings.forwardSpeed,
            0f,
            m_RigidBody.velocity.normalized.z * movementSettings.forwardSpeed);
        m_RigidBody.AddForce(transform.forward * dashForce);
        isDashing = true;
        canDash = false;
        m_RigidBody.drag = movementSettings.dashDrag;
        GetComponent<TrailRenderer>().enabled = true;
        yield return new WaitForSeconds(dashDuration);
        m_RigidBody.velocity = prevVelocity;
        isDashing = false;
        GetComponent<TrailRenderer>().enabled = false;
        PersonalUI.instance.StartDashTimer(dashCooldown);
        yield return new WaitForSeconds(dashCooldown);
        SoundManager.instance.PlayDashCooldownFinished();
        canDash = true;
    }

    private IEnumerator InitiateDash2()
    {
        PlayDashSound(GetComponent<PlayerID>().playerID);
        CmdPlayDashSound(GetComponent<PlayerID>().playerID);
        chrome.enabled.value = true;
        Vector2 input = GetRawInput();

        if (input == Vector2.zero)
            input = Vector2.up;
        m_RigidBody.useGravity = false;
        m_RigidBody.velocity = Vector3.zero;
        m_RigidBody.AddForce(((transform.forward * input.y) + (transform.right * input.x)) * dashForce);
        isDashing = true;
        canDash = false;
        m_RigidBody.drag = movementSettings.dashDrag;
        GetComponent<TrailRenderer>().enabled = true;

        yield return new WaitForSeconds(dashDuration);

        chrome.enabled.value = false;
        m_RigidBody.useGravity = true;
        m_RigidBody.velocity = m_RigidBody.velocity.normalized * movementSettings.currentTargetSpeed;
        isDashing = false;
        GetComponent<TrailRenderer>().enabled = false;
        PersonalUI.instance.StartDashTimer(dashCooldown);
        yield return new WaitForSeconds(dashCooldown);
        SoundManager.instance.PlayDashCooldownFinished();
        canDash = true;
    }

    private IEnumerator StartShootCooldown()
    {
        _shootCooldownDone = false;
        PersonalUI.instance.StartShootTimer(shootCooldown);
        yield return new WaitForSeconds(shootCooldown);
        SoundManager.instance.PlayLaserCooldownFinished();
        _shootCooldownDone = true;
        yield return 0;
    }

    private void ChargingShot()
    {
        if (!chargeSource.isPlaying) {
            CmdPlayChargingShot(GetComponent<PlayerID>().playerID);
            CmdThirdPersonCharge();
        }
        beamDistance += Time.deltaTime * beamDistanceMultiplier;
        beamDistance = Mathf.Clamp(beamDistance, beamMinDistance, beamMaxDistance);
        m_RigidBody.velocity = m_RigidBody.velocity * (1f / (1f + (beamDistance * beamSlowMultiplier)));

        float scaleValue = (beamDistance / beamMaxDistance) * 0.01f;
        firstPersonChargeEffect.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        PersonalUI.instance.UpdateShootCharge(beamDistance, beamMaxDistance);
        Debug.DrawRay(beamOrigin.position, beamOrigin.forward * beamDistance, Color.blue, 0.1f);
    }
    
    private IEnumerator ThirdPersonCharge()
    {
       
        while (thirdPersonChargeEffect.transform.localScale.x <= 0.01f) {
            float scaleValue = (beamDistanceMultiplier / beamMaxDistance) * 0.01f * Time.deltaTime;
            thirdPersonChargeEffect.transform.localScale += new Vector3(scaleValue, scaleValue, scaleValue);
            yield return 0;
        }
        yield return 0;
    }

    [Command]
    private void CmdThirdPersonCharge()
    {
        RpcThirdPersonCharge();
    }

    [ClientRpc]
    private void RpcThirdPersonCharge()
    {

        if (!isLocalPlayer)
        {
            if (thirdPersonCharge != null)
                StopCoroutine(thirdPersonCharge);
            thirdPersonChargeEffect.transform.localScale = Vector3.zero;
            thirdPersonCharge = StartCoroutine(ThirdPersonCharge());
        }
    }

    [Command]
    private void CmdStopThirdPersonCharge()
    {
        RpcStopThirdPersonCharge();
    }

    [ClientRpc]
    private void RpcStopThirdPersonCharge()
    {
        StopEffects();
    }

    public void StopEffects()
    {
        if (!isLocalPlayer)
        {
            if (thirdPersonCharge != null)
                StopCoroutine(thirdPersonCharge);
            thirdPersonChargeEffect.transform.localScale = Vector3.zero;
        } else
        {
            firstPersonChargeEffect.transform.localScale = Vector3.zero;
        }
        if (chargeSound != null)
            StopCoroutine(chargeSound);
        chargeSource.Stop();
    }

    private void ShootSphereCast()
    {
        int iD = GetComponent<PlayerID>().playerID;
        CmdFireSound(iD);
        GetComponent<MaterialSwap>().TurnVisibleInstant();
        RaycastHit hit;
        CmdAddTotalShot(myTeamID);
        beam.SetActive(true);
        Vector3 startPosition = (beamOrigin.position - new Vector3(0, 0.2f, 0));
        beam.GetComponent<LineRenderer>().SetPosition(0, startPosition);
        float finalDistance = 0;

        if (Physics.SphereCast(beamOrigin.position, sphereCastWidth, beamOrigin.forward, out hit, beamDistance))
        {

            if (OnShoot != null)
                OnShoot();

            if (hit.collider && hit.collider.gameObject.CompareTag("Player"))
            {
                CmdPlayerIDToKill(hit.transform.GetComponent<PlayerID>().playerID);
                Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hit.distance, Color.red, 1f);
                CmdPlayDeathSound(hit.transform.GetComponent<PlayerID>().playerID);
                PersonalUI.instance.StartCoroutine(PersonalUI.instance.ShowHitMarker());
                finalDistance = hit.distance;
                SoundManager.instance.PlayLaserHit();
                
                CmdCallAddPoint(myTeamID);

            }
            else if (hit.collider && hit.collider.gameObject.CompareTag("Decoy"))
            {
                Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hit.distance, Color.red, 1f);
                if (hit.collider.gameObject.GetComponent<DecoyBehaviour>() != null)
                {
                    hit.collider.gameObject.GetComponent<DecoyBehaviour>().Death();
                    CmdDecoyDeath(hit.collider.gameObject.GetComponent<NetworkIdentity>().netId);
                }
                else
                    hit.collider.gameObject.GetComponent<DummyBehaviour>().Death();

                finalDistance = hit.distance;
                SoundManager.instance.PlayLaserHit();
            }
            else if (hit.collider)
            {
                Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hit.distance, Color.red, 1f);
                finalDistance = hit.distance;

            }
            else
            {
                Debug.DrawRay(beamOrigin.position, beamOrigin.forward * beamDistance, Color.red, 1f);
            }
        }

        else
        {
            finalDistance = beamDistance;
        }

        Vector3 endPosition = beamOrigin.position + beamOrigin.forward * finalDistance;

        beam.GetComponent<LineRenderer>().SetPosition(1, endPosition);

        CmdCreateBeam(startPosition, endPosition);
        StartCoroutine(HideBeam(1f));

        StartCoroutine(StartShootCooldown());
        beamDistance = 0;
        firstPersonChargeEffect.transform.localScale = Vector3.zero;
        CmdStopThirdPersonCharge();
        chargingShot = false;
    }

    IEnumerator HideBeam(float timer)
    {
        yield return new WaitForSeconds(timer);
        CmdHideBeam();
        beam.SetActive(false);
        yield return 0;
    }

    [Command]
    public void CmdDecoyDeath(NetworkInstanceId decoy)
    {
        RpcDecoyDeath(decoy);
    }


    [ClientRpc]
    private void RpcDecoyDeath(NetworkInstanceId decoyID)
    {
        if (!isLocalPlayer)
        {
            GameObject decoy = ClientScene.FindLocalObject(decoyID);
            print(decoyID);
            print(decoy);
            print(decoy.transform.name);

            decoy.GetComponent<DecoyBehaviour>().Death();
            //decoy.Death();
        }

    }

    [Command]
    private void CmdCreateBeam(Vector3 startPosition, Vector3 endPosition)
    {
        RpcCreateBeam(startPosition, endPosition);
    }

    [ClientRpc]
    private void RpcCreateBeam(Vector3 startPosition, Vector3 endPosition)
    {
        beam.SetActive(true);
        beam.GetComponent<LineRenderer>().SetPosition(0, startPosition);
        beam.GetComponent<LineRenderer>().SetPosition(1, endPosition);
    }

    [Command]
    private void CmdHideBeam()
    {
        RpcHideBeam();
    }

    [ClientRpc]
    private void RpcHideBeam()
    {
        beam.SetActive(false);
    }

    [Command]
    private void CmdPlayerIDToKill(int enemyID)
    {
        RpcPlayerIDToKill(enemyID);
    }

    [ClientRpc]
    private void RpcPlayerIDToKill(int enemyID)
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player"); ///SERVER STAT MANAGER DOES NOT WORK

        foreach (GameObject player in playerList)
        {
            if (enemyID == player.GetComponent<PlayerID>().playerID)
            {
                player.GetComponent<PlayerController>().Death();
                //RoundManager.instance.RemovePointsOnPlayer(player.GetComponent<PlayerController>().myTeamID);
            }
        }
    }

    [Command]
    private void CmdPlayChargingShot(int id) {
        RpcPlayChargingShot(id);
    }

    [ClientRpc]
    private void RpcPlayChargingShot(int id)
    {

        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player"); ///SERVER STAT MANAGER DOES NOT WORK

        foreach (GameObject player in playerList)
        {
            if (id == player.GetComponent<PlayerID>().playerID)
            {
                player.GetComponent<PlayerController>().chargeSource.Play();
                player.GetComponent<PlayerController>().chargeSource.volume = 0f;
                chargeSound = StartCoroutine(ChargeVolume(player));
            }
        }
    }

    private IEnumerator ChargeVolume(GameObject player)
    {
        AudioSource source = player.GetComponent<PlayerController>().chargeSource;
        for (float i = 0; i < (beamMaxDistance / beamDistanceMultiplier); i += Time.deltaTime / (beamMaxDistance / beamDistanceMultiplier))
        {
            source.volume = i;
            source.volume = Mathf.Clamp(source.volume, 0.1f, 1f);
            yield return 0;
        }


        yield return 0;
    }

    public void FireSound(int playerID) {
        SoundManager.instance.PlayFireLaser(playerID);
    }

    private void StopCharge(int id) {

        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player"); ///SERVER STAT MANAGER DOES NOT WORK

        foreach (GameObject player in playerList) {
            if (id == player.GetComponent<PlayerID>().playerID) {
                player.GetComponent<PlayerController>().chargeSource.Stop();
            }
        }
    }

    [Command]
    public void CmdFireSound(int playerID) {
        RpcPlayFireSound(playerID);
    }

    [ClientRpc]
    public void RpcPlayFireSound(int playerID) {
        FireSound(playerID);
        StopCharge(playerID);
    }

    public void PlayNewAreaSound(bool invisible) {
        if(isLocalPlayer)
        SoundManager.instance.PlayNewArea(invisible);
    }

    public void Death()
    {

        if (OnDeath != null)
            OnDeath();

        StartCoroutine(RespawnTimer());


        if (isLocalPlayer)
        {
            StartCoroutine(DeathTimer());
            CmdStopThirdPersonCharge();
        }
        else
        {

            //Kill other player
        }


    }

    private IEnumerator DeathTimer()
    {
        canDash = false; canMove = false; canShoot = false;
        isDead = true;
        PersonalUI.instance.deathText.enabled = true;
        cam.depth = -1;
        CmdAddDeathTotal(myTeamID);
        yield return new WaitForSeconds(RoundManager.instance.deathTimer);

        canDash = true; canMove = true; canShoot = true;
        PersonalUI.instance.deathText.enabled = false;
        PlayerSpawnManager.instance.Spawn(this.gameObject);
        isDead = false;
        cam.depth = 1;

        yield return 0;
    }

    private void DeathSound(int id) {
        SoundManager.instance.PlayDeathSound(id);
    }

    [Command]
    private void CmdPlayDeathSound(int id) {
        RpcPlayDeathSound(id);
    }

    [ClientRpc]
    private void RpcPlayDeathSound(int id) {
        DeathSound(id);
    }

    private IEnumerator RespawnTimer()
    {
        isDead = true;
        yield return new WaitForSeconds(RoundManager.instance.deathTimer);
        if (OnRespawn != null)
            OnRespawn();
        isDead = false;
        yield return 0;
    }

    [Command]
    public void CmdSendSpawnLocation(Vector3 pos)
    {
        RpcSendSpawnLocation(pos);
    }

    [ClientRpc]
    public void RpcSendSpawnLocation(Vector3 pos)
    {
        transform.position = pos;
        
    }

    private float SlopeMultiplier()
    {
        float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
        return movementSettings.slopeCurveModifier.Evaluate(angle);
    }


    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) +
                               advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
            }
        }
    }


    private Vector2 GetInput()
    {
        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),    //CrossPlatformInputManager.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")      //CrossPlatformInputManager.GetAxis("Vertical")
        };   

        movementSettings.UpdateDesiredTargetSpeed(input);
        return input;
    }

    private Vector2 GetRawInput()
    {
        Vector2 input = new Vector2
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };

        movementSettings.UpdateDesiredTargetSpeed(input);
        return input;
    }


    private void RotateView()
    {

        if (isDead)
            return;

        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        mouseLook.LookRotation(transform, cam.transform);

        if (isGrounded || advancedSettings.airControl)
        {
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
        }
    }

    /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
    private void GroundCheck()
    {
        wasPreviouslyGrounded = isGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo, ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        //if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, ((m_Capsule.height / 2f) + m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)  )
        {
            isGrounded = true;
            m_GroundContactNormal = hitInfo.normal;
        }
        else
        {
            isGrounded = false;
            m_GroundContactNormal = Vector3.up;
            airTime += (Time.deltaTime * 0.5f);
        }

        if (!wasPreviouslyGrounded && isGrounded)
        {
            if (isJumping)
                isJumping = false;
            if (airTime > advancedSettings.airTimeLimit)
                SoundManager.instance.PlayLandingSound(airTime);
            airTime = 0;
        }
    }

    private void RunMan() 
    {
        
        if (!isGrounded || Velocity.magnitude <= 4)
        {
            runSource.Stop();
            CmdRunMan(false);
        } else
        if (isGrounded && Velocity.magnitude >= 4 && !runSource.isPlaying)
        {
            runSource.Play();
            CmdRunMan(true);
        }
    }

    [Command]
    private void CmdRunMan(bool play)
    {
        RpcRunMan(play);
    }

    [ClientRpc]
    private void RpcRunMan(bool play)
    {
        if (!isLocalPlayer)
        {
            if (play)
            {
                runSource.Play();
            }
            else
            {
                runSource.Stop();
            }
        }
    }

    [Command]
    private void CmdCallAddPoint(int teamID)
    {
        RpcCallAddPoint(teamID);
        
    }

    [ClientRpc]
    private void RpcCallAddPoint(int teamID)
    {
        SharedUI.instance.PointAnimation(teamID);
        if (isServer)
        {
            RoundManager.instance.AddPoint(myTeamID, 1);
        }
    }

    [Command]
    private void CmdAddTotalShot(int teamID)
    {
        RpcAddTotalShot(teamID);

    }

    [ClientRpc]
    private void RpcAddTotalShot(int teamID)
    {
        if (isServer)
        {
            TABScoreManager.instance.IncreaseShots(myTeamID);
        }
    }

    [Command]
    private void CmdAddDeathTotal(int teamID)
    {
        RpcAddDeathTotal(teamID);

    }

    [ClientRpc]
    private void RpcAddDeathTotal(int teamID)
    {
        if (isServer)
        {
            TABScoreManager.instance.IncreaseDeaths(myTeamID);
        }
    }
}
