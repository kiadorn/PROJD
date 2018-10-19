using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : NetworkBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public float groundedDrag = 5f;
            public float jumpDrag = 0f;
            public float dashDrag = 0f;
            public float slowDownLimit = 0.75f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;


            private bool m_Running;
            private GameObject SM;

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed = StrafeSpeed;
                }
                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed = BackwardSpeed;
                }
                if (input.y > 0)
                {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }
                if (Input.GetKey(RunKey))
                {
                    //CurrentTargetSpeed *= RunMultiplier;

                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
            }

            public bool Running
            {
                get { return m_Running; }
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
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        public Camera cam;
        public Camera deathCamera;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        public Behaviour[] componentsToDisable;

        [SyncVar]
        public float movementUpdateRate;

        [Header("Shooting")]
        public Transform beamOrigin;
        public float beamDistanceMultiplier = 1f;
        public float beamMaxDistance;
        private float _beamDistance;
        public float shootCooldown;

        [Header("Dash")]
        public float dashDuration;
        public float dashForce;
        public float dashCooldown;

        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, isGrounded, dashing, isDead;

        private Vector3 _lastPosition;
        private Vector3 _mylastPosition;
        private Quaternion _lastRotation;
        private bool _shootCooldownDone = true;
        private bool _chargingShoot = false;

        public Team myTeam;
        public int myTeamID;
        public bool canMove = false;
        public bool canShoot = false;
        public bool canDash = false;

        private ServerStatsManager serverStats;

        public delegate void ControllerEvent();
        public event ControllerEvent OnStartJump;

        public event ControllerEvent OnDeath;

        public event ControllerEvent OnRespawn;

        public event ControllerEvent OnShoot;

        [Header("UI")]

        public Sprite[] UIChanges;
        
        public float forwardRate;
        public float strafeRate;


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
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
				return movementSettings.Running;
            }
        }
        
        public bool Dashing
        {
            get
            {
                return dashing;
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
            } else
            {
                transform.gameObject.layer = 2;
            }
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);

            SoundManager.instance.AddSoundOnStart(this);


        }


        private void Update()
        {

            if (isLocalPlayer)
            {
                RotateView();

                if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump && !dashing)
                {
                    m_Jump = true;
                    //GetComponent<AudioSource>().Play(); //GAMMAL TEST
                    //CmdPlaySound();
                }

                if (_mylastPosition != transform.position) //Ändra till 0.1 skillnad 
                {
                    CmdUpdatePosition(transform.position);
                    _mylastPosition = transform.position;
                }
                
                if (_lastRotation != transform.rotation)
                {
                    CmdUpdateRotation(transform.rotation);
                    _lastRotation = transform.rotation;
                }

                
            } else
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
        private void CmdPlaySound() {
            RpcPlaySound();
        }

        [ClientRpc]
        private void RpcPlaySound() {
            //GetComponent<AudioSource>().Play(); //GAMMAL TEST
            if(!isLocalPlayer)
                SoundManager.instance.PlayJumpSound();
        }



        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                if (!dashing)
                {
                    GroundCheck();
                    Movement();
                }
                ShootCheck();

                if (Input.GetKey(KeyCode.LeftShift) && canDash && canMove && !_chargingShoot)
                {
                    StartCoroutine(InitiateDash());
                }
            } else
            {
                transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * movementUpdateRate);
            }
        }

        private void Movement()
        {
            Vector2 input = GetRawInput();//GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || isGrounded) && canMove)
            {
                
                // always move along the camera forward as it is the direction that it being aimed at
                //Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                forwardRate = Mathf.Lerp(forwardRate, input.y, Time.deltaTime * advancedSettings.speedUpRate);
                strafeRate = Mathf.Lerp(strafeRate, input.x, Time.deltaTime * advancedSettings.speedUpRate);


                Vector3 desiredMove = cam.transform.forward * forwardRate + cam.transform.right * strafeRate;

                //print(cam.transform.forward * forwardRate + " " + cam.transform.right * strafeRate);

                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                //print("Vel: " + Velocity.ToString() + " Rates: " + forwardRate + " " + strafeRate + " DesMov: " + desiredMove.ToString());

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (isGrounded)
            {
                m_RigidBody.drag = movementSettings.groundedDrag;

                if (m_Jump)
                {
                    m_RigidBody.drag = movementSettings.jumpDrag;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;

                    if (OnStartJump != null)
                        OnStartJump();
                    
                }

                //if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
                //{
                //    m_RigidBody.velocity = 0;
                //}

                if (!m_Jumping && Mathf.Abs(input.x) < movementSettings.slowDownLimit && Mathf.Abs(input.y) < movementSettings.slowDownLimit)
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
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
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
                } else if (!Input.GetButton("Fire1") && _chargingShoot)
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
                } else
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
                    serverStats.dashEmpty.sprite = UIChanges[1];
                    serverStats.dashFill.sprite = UIChanges[2];
                    serverStats.shootEmpty.sprite = UIChanges[3];
                    serverStats.shootFill.sprite = UIChanges[4];

                }
                else
                {
                    AssignTeamWhite();
                }
            }
        }

        private void AssignTeamWhite()
        {
            myTeam = Team.White;
            myTeamID = 1;
            GetComponent<MaterialSwap>().team = MaterialSwap.Team.light;
            //GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.white;
            GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = Color.black;
            GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = Color.white;
        }

        private void AssignTeamBlack()
        {
            myTeam = Team.Black;
            myTeamID = 2;
            GetComponent<MaterialSwap>().team = MaterialSwap.Team.dark;
            //GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.black;
            GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = Color.white;
            GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = Color.black;

           
        }

        private IEnumerator InitiateDash()
        {
            //Vector3 prevVelocity = m_RigidBody.velocity;
            Vector3 prevVelocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
            m_RigidBody.velocity = transform.forward * dashForce;
            dashing = true;
            canDash = false;
            m_RigidBody.drag = movementSettings.dashDrag;
            GetComponent<TrailRenderer>().enabled = true;
            yield return new WaitForSeconds(dashDuration);
            m_RigidBody.velocity = prevVelocity;
            dashing = false;
            GetComponent<TrailRenderer>().enabled = false;
            serverStats.StartDashTimer(dashCooldown);
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
        
        private IEnumerator StartShootCooldown()
        {
            _shootCooldownDone = false;
            serverStats.StartShootTimer(shootCooldown);
            yield return new WaitForSeconds(shootCooldown);
            _shootCooldownDone = true;
            yield return 0;
        }

        private void ChargingShot()
        {
            _chargingShoot = true;

            _beamDistance += Time.deltaTime * beamDistanceMultiplier;
            if (_beamDistance > beamMaxDistance)
            {
                _beamDistance = beamMaxDistance;
            }
            m_RigidBody.velocity = m_RigidBody.velocity * (1f / (1f + (_beamDistance * 0.08f)));

            serverStats.UpdateShootCharge(_beamDistance, beamMaxDistance);
            Debug.DrawRay(beamOrigin.position, beamOrigin.forward * _beamDistance, Color.blue, 0.1f);
        }

        private void ShootSphereCastAll()
        {
            if (_beamDistance > beamMaxDistance)
            {
                _beamDistance = beamMaxDistance;
            }
            RaycastHit[] hits = Physics.SphereCastAll(beamOrigin.position, 0.25f, beamOrigin.forward, _beamDistance);
            Debug.Log("Firing!");
            bool hitSomething = false;
            for(int i = hits.Length - 1; i >= 0; i--)
            //foreach (RaycastHit hit in hits)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                sphere.transform.position = beamOrigin.position + beamOrigin.forward * hits[i].distance;
                sphere.GetComponent<SphereCollider>().enabled = false;
                if (hits[i].collider && hits[i].collider.gameObject.CompareTag("Player"))
                {
                    hitSomething = true;
                    Debug.Log("HIT PLAYER");
                    Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hits[i].distance, Color.red, 1f);

                    break;
                }
                else if (hits[i].collider)
                {
                    hitSomething = true;
                    Debug.Log("HIT SOMETHING ELSE: " + hits[i].collider.name);
                    Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hits[i].distance, Color.red, 1f);

                    break;
                }
            }

            if (!hitSomething)
            {
                Debug.Log("HIT NOTHING");
                Debug.DrawRay(beamOrigin.position, beamOrigin.forward * _beamDistance, Color.red, 1f);
            }
            
            _beamDistance = 0;
            _chargingShoot = false;

        }

        private void ShootSphereCast()
        {
            
            RaycastHit hit;
            Debug.Log("Firing!");


            if (Physics.SphereCast(beamOrigin.position, 0.25f, beamOrigin.forward, out hit, _beamDistance)) { 

                //Skapar bara en sphere så vi vet vart vi träffade.
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                sphere.transform.position = beamOrigin.position + beamOrigin.forward * hit.distance;
                sphere.GetComponent<SphereCollider>().enabled = false;

                if (OnShoot != null)
                    OnShoot();

                if (hit.collider && hit.collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("HIT PLAYER");
                    CmdPlayerIDToKill(hit.transform.GetComponent<PlayerID>().playerID);
                    Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hit.distance, Color.red, 1f);

                }
                else if (hit.collider)
                {
                    Debug.Log("HIT SOMETHING ELSE: " + hit.collider.name);
                    Debug.DrawRay(beamOrigin.position, beamOrigin.forward * hit.distance, Color.red, 1f);

                } else
                {
                    Debug.DrawRay(beamOrigin.position, beamOrigin.forward * _beamDistance, Color.red, 1f);

                }

            } else {
                Debug.Log("SphereCast suger");
            }

            StartCoroutine(StartShootCooldown());
            _beamDistance = 0;
            _chargingShoot = false;
        }

        [Command]
        private void CmdPlayerIDToKill(int enemyID)
        {
            RpcPlayerIDToKill(enemyID);
        }

        [ClientRpc]
        private void RpcPlayerIDToKill(int enemyID)
        {
            serverStats.AddPoint(myTeamID);
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in playerList)
            {
                if (enemyID == player.GetComponent<PlayerID>().playerID)
                {
                    player.GetComponent<RigidbodyFirstPersonController>().Death();
                }
            }
        }

        public void Death()
        {
            if (OnDeath != null)
                OnDeath();

            StartCoroutine(RespawnTimer());

            if (isLocalPlayer)
            {
                //GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                

                isDead = true;
                deathCamera.enabled = true;
                StartCoroutine(DeathTimer());
            } else
            {
                //GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
                Debug.Log(transform.name + " HAS DIED");
                //Kill other player
            }


        }

        private IEnumerator DeathTimer()
        {
            canDash = false; canMove = false; canShoot = false;
            //UI YOU HAVE DIED;
            serverStats.DEAD.enabled = true;
            yield return new WaitForSeconds(serverStats.deathTimer);
            canDash = true; canMove = true; canShoot = true;
            serverStats.DEAD.enabled = false;
            SpawnManager.instance.Spawn(this.gameObject);

            

            isDead = false;
            deathCamera.enabled = false;

            //UI YOU HAVE NOT DIED, YOU HAVE UNDIEDED;
            yield return 0;
        }

        private IEnumerator RespawnTimer()
        {
            yield return new WaitForSeconds(serverStats.deathTimer);
            if (OnRespawn != null)
                OnRespawn();
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
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
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

            mouseLook.LookRotation (transform, cam.transform);

            if (isGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = isGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                isGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && isGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
