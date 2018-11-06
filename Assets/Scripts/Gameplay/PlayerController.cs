using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

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
    private Coroutine thirdPersonCharge, thirdPersonChargeSound;

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


    private ServerStatsManager serverStats;

    public delegate void ControllerEvent();
    //public delegate void ControllerIDEvent(int playerID);
    public event ControllerEvent OnStartJump;
    //[SyncEvent]
    public event ControllerEvent EventOnDeath;
    //[SyncEvent]
    public event ControllerEvent EventOnRespawn;
    public event ControllerEvent OnShoot;
    //public event ControllerIDEvent OnDash;

    [Header("UI")]

    public Sprite[] UIChanges;
    public TeamAsset teamWhiteAsset;
    public TeamAsset teamBlackAsset;
    public TeamAsset myAsset;

    public float forwardRate;
    public float strafeRate;

    public AudioSource runSource;
    public enum Team
    {
        White = 1,
        Black
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
        serverStats = ServerStatsManager.instance;
        AssignTeam();
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
            SoundManager.instance.AddSoundOnStart(this);
            SoundManager.instance.SetPlayerOrigin(gameObject);
            GetComponent<MaterialSwap>().thirdPersonModel.gameObject.layer = 9;
            GetComponent<MaterialSwap>().thirdPersonMask.gameObject.layer = 9;
            thirdPersonChargeEffect.SetActive(false);

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
            RunMan();
        }
        else
        {
            transform.rotation = _lastRotation;
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Death"))
        {
            Death();
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
            ShootCheck();

            if (Input.GetKey(KeyCode.LeftShift) && canDash && canMove && !chargingShot)
            {
                StartCoroutine(InitiateDash());
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
            //float speedUpRate = isGrounded ? advancedSettings.speedUpRate : advancedSettings.airSpeedUpRate;
            // always move along the camera forward as it is the direction that it being aimed at
            //Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            forwardRate = Mathf.Lerp(forwardRate, input.y, Time.deltaTime * advancedSettings.speedUpRate);
            strafeRate = Mathf.Lerp(strafeRate, input.x, Time.deltaTime * advancedSettings.speedUpRate);


            Vector3 desiredMove = cam.transform.forward * forwardRate + cam.transform.right * strafeRate;

            //print(cam.transform.forward * forwardRate + " " + cam.transform.right * strafeRate);

            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
            if (!isGrounded) desiredMove *= advancedSettings.airSpeedUpRate;

            //print("Vel: " + Velocity.ToString() + " Rates: " + forwardRate + " " + strafeRate + " DesMov: " + desiredMove.ToString());

            desiredMove.x = desiredMove.x * movementSettings.currentTargetSpeed;
            desiredMove.z = desiredMove.z * movementSettings.currentTargetSpeed;
            desiredMove.y = desiredMove.y * movementSettings.currentTargetSpeed;
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

                if (OnStartJump != null)
                    OnStartJump();

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
                StickToGroundHelper();
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
            if (Input.GetButton("Fire1"))
            {
                ChargingShot();
            }
            else if (!Input.GetButton("Fire1") && chargingShot)
            {
                ShootSphereCast();
                //ShootSphereCastAll();
            }
        }
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

                serverStats.crosshair.sprite = UIChanges[0];
                //serverStats.dashEmpty.sprite = UIChanges[1];
                //serverStats.dashFill.sprite = UIChanges[2];
                //serverStats.shootEmpty.sprite = UIChanges[3];
                //serverStats.shootFill.sprite = UIChanges[4];

            }
            else
            {
                AssignTeamWhite();
            }
        }
        AssignParticleColor();
    }

    private void AssignTeamWhite()
    {
        myTeam = Team.White;
        myTeamID = 1;
        myAsset = teamWhiteAsset;

    }

    private void AssignTeamBlack()
    {
        myTeam = Team.Black;
        myTeamID = 2;
        myAsset = teamBlackAsset;
    }

    private void AssignParticleColor()
    {
        ParticleSystem.ColorOverLifetimeModule col = GetComponent<MaterialSwap>().invisibleTrail.colorOverLifetime; 
        col.color = myAsset.particleGradient;
    }

    public void PlayDashSound(int playerID) {
        SoundManager.instance.PlayDashSound(playerID);
    }

    [Command]
    public void CmdPlayDashSound(int playerID) {
        print("Command sent");
        //if (isServer) {
        //    print("Approved by server");
            RpcPlayDashSound(playerID);

        //}
    }

    [ClientRpc]
    public void RpcPlayDashSound(int playerID) {
        print("Server sent");
        if (!isLocalPlayer) {
            print(playerID.ToString() + " is playing sound");
            PlayDashSound(playerID);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Objective") && isLocalPlayer) {
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
        serverStats.StartDashTimer(dashCooldown);
        yield return new WaitForSeconds(dashCooldown);
        SoundManager.instance.PlayDashCooldownFinished();
        canDash = true;
    }

    private IEnumerator StartShootCooldown()
    {
        _shootCooldownDone = false;
        serverStats.StartShootTimer(shootCooldown);
        yield return new WaitForSeconds(shootCooldown);
        SoundManager.instance.PlayLaserCooldownFinished();
        _shootCooldownDone = true;
        yield return 0;
    }

    private void ChargingShot()
    {
        chargingShot = true;
        if (!transform.GetChild(0).gameObject.GetComponent<AudioSource>().isPlaying) {
            CmdPlayChargingShot(GetComponent<PlayerID>().playerID);
            CmdThirdPersonCharge();
        }
        beamDistance += Time.deltaTime * beamDistanceMultiplier;
        beamDistance = Mathf.Clamp(beamDistance, beamMinDistance, beamMaxDistance);
        m_RigidBody.velocity = m_RigidBody.velocity * (1f / (1f + (beamDistance * beamSlowMultiplier)));

        float scaleValue = (beamDistance / beamMaxDistance) * 0.01f;
        firstPersonChargeEffect.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        serverStats.UpdateShootCharge(beamDistance, beamMaxDistance);
        Debug.DrawRay(beamOrigin.position, beamOrigin.forward * beamDistance, Color.blue, 0.1f);
    }
    
    private IEnumerator ThirdPersonCharge()
    {
        float scaleValue = (beamDistanceMultiplier / beamMaxDistance) * 0.01f;
        while (thirdPersonChargeEffect.transform.localScale.x <= 0.01f) {
            thirdPersonChargeEffect.transform.localScale += new Vector3(scaleValue, scaleValue, scaleValue);
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
            if (thirdPersonChargeSound != null)
                StopCoroutine(thirdPersonChargeSound);
            thirdPersonChargeEffect.transform.localScale = Vector3.zero;
        }
        transform.GetChild(0).GetComponent<AudioSource>().Stop();
    }

    //private void ShootSphereCastAll()
    //{
    //    if (_beamDistance > beamMaxDistance)
    //    {
    //        _beamDistance = beamMaxDistance;
    //    }
    //    RaycastHit[] hits = Physics.SphereCastAll(beamOrigin.position, 0.25f, beamOrigin.forward, _beamDistance);
    //    Debug.Log("Firing!");
    //    bool hitSomething = false;
    //    for (int i = hits.Length - 1; i >= 0; i--)
    //    //foreach (RaycastHit hit in hits)
    //    {
    //        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //        sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    //        sphere.transform.position = beamOrigin.position + beamOrigin.forward * hits[i].distance;
    //        sphere.GetComponent<SphereCollider>().enabled = false;
    //        if (hits[i].collider && hits[i].collider.gameObject.CompareTag("Player"))
    //        {
    //            hitSomething = true;
    //            Debug.Log("HIT PLAYER");
    //            Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hits[i].distance, Color.red, 1f);

    //            break;
    //        }
    //        else if (hits[i].collider)
    //        {
    //            hitSomething = true;
    //            Debug.Log("HIT SOMETHING ELSE: " + hits[i].collider.name);
    //            Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hits[i].distance, Color.red, 1f);

    //            break;
    //        }
    //    }

    //    if (!hitSomething)
    //    {
    //        Debug.Log("HIT NOTHING");
    //        Debug.DrawRay(beamOrigin.position, beamOrigin.forward * _beamDistance, Color.red, 1f);
    //    }

    //    _beamDistance = 0;
    //    _chargingShoot = false;

    //}

    private void ShootSphereCast()
    {
        int iD = GetComponent<PlayerID>().playerID;
        //FireSound(iD);
        CmdFireSound(iD);
        RaycastHit hit;

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
                serverStats.StartCoroutine(serverStats.ShowHitMarker());
                finalDistance = hit.distance;

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
                serverStats.RemovePointsOnPlayer(player.GetComponent<PlayerController>().myTeamID);
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
            AudioSource source = player.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
            if (id == player.GetComponent<PlayerID>().playerID)
            {
                player.transform.GetChild(0).gameObject.GetComponent<AudioSource>().Play();
                source.volume = 0f;
                source.Play();
                thirdPersonChargeSound = StartCoroutine(ChargeVolume(player));
            }
        }
    }

    private IEnumerator ChargeVolume(GameObject player)
    {
        AudioSource source = player.transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        source.volume = 0.1f;
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
                player.transform.GetChild(0).gameObject.GetComponent<AudioSource>().Stop();
            }
        }
    }

    [Command]
    public void CmdFireSound(int playerID) {
        print("Command sent");
        //if (isServer) {
        //    print("Approved by server");
        RpcPlayFireSound(playerID);

        //}
    }

    [ClientRpc]
    public void RpcPlayFireSound(int playerID) {
        print("Server sent");
        //if (!isLocalPlayer) {
            print(playerID.ToString() + " is playing sound");
            FireSound(playerID);
            StopCharge(playerID);
        //}
    }

    public void PlayNewAreaSound(bool invisible) {
        if(isLocalPlayer)
        SoundManager.instance.PlayNewArea(invisible);
    }

    public void Death()
    {

        if (EventOnDeath != null)
            EventOnDeath();

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
        serverStats.DEAD.enabled = true;
        cam.depth = -1;

        yield return new WaitForSeconds(serverStats.deathTimer);

        canDash = true; canMove = true; canShoot = true;
        serverStats.DEAD.enabled = false;
        SpawnManager.instance.Spawn(this.gameObject);
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
        yield return new WaitForSeconds(serverStats.deathTimer);
        if (EventOnRespawn != null)
            EventOnRespawn();
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
            x = CrossPlatformInputManager.GetAxis("Horizontal"),
            y = CrossPlatformInputManager.GetAxis("Vertical")
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
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
            m_GroundContactNormal = hitInfo.normal;
        }
        else
        {
            isGrounded = false;
            m_GroundContactNormal = Vector3.up;
        }
        if (!wasPreviouslyGrounded && isGrounded && isJumping)
        {
            isJumping = false;
        }
    }

    private void RunMan() 
    {
        
        if (!isGrounded || Velocity.magnitude <= 4)
        {
            runSource.Stop();
            CmdRunMan(false);
        }
        if (Velocity.magnitude >= 4 && !runSource.isPlaying)
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
}
