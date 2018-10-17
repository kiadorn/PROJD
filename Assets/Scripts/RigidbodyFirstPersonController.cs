using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
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
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;


            private bool m_Running;

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
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        public Camera cam;
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

        [Header("Dash")]
        public float dashDuration;
        public float dashForce;
        public float dashCooldown;

        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded, dashing;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private bool _shootCooldownDone = true;
        private bool _chargingShoot = false;
        
        public Team myTeam;
        public int myTeamID;
        public bool canMove = false;
        public bool canShoot = false;
        public bool canDash = false;
        
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
            get { return m_IsGrounded; }
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


        private void Start()
        {
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
        }


        private void Update()
        {

            if (isLocalPlayer)
            {
                RotateView();

                if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump && !dashing)
                {
                    m_Jump = true;
                    GetComponent<AudioSource>().Play();
                    CmdPlaySound();
                }

                CmdUpdateRotation(transform.rotation);
            } else
            {
                transform.rotation = _lastRotation;
            }
        }

        [Command]
        private void CmdPlaySound() {
            RpcPlaySound();
        }

        [ClientRpc]
        private void RpcPlaySound() {
            GetComponent<AudioSource>().Play();
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

                if (Input.GetKey(KeyCode.LeftShift) && canDash && canMove)
                {
                    StartCoroutine(InitiateDash());

                }

                CmdUpdatePosition(transform.position);
            } else
            {
                transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * movementUpdateRate);
            }
        }

        private void Movement()
        {
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded) && canMove)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
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
                } else
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
            Vector3 prevVelocity = m_RigidBody.velocity;
            m_RigidBody.velocity = transform.forward * dashForce;
            dashing = true;
            canDash = false;
            m_RigidBody.drag = 0f;
            GetComponent<TrailRenderer>().enabled = true;
            yield return new WaitForSeconds(dashDuration);
            m_RigidBody.velocity = prevVelocity;
            dashing = false;
            GetComponent<TrailRenderer>().enabled = false;
            ServerStatsManager.instance.StartDashTimer(dashCooldown);
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }

        private void ChargingShot()
        {
            _chargingShoot = true;
            _beamDistance += Time.deltaTime * beamDistanceMultiplier;
            ServerStatsManager.instance.UpdateShootCharge(_beamDistance, beamMaxDistance);
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
            if (_beamDistance > beamMaxDistance)
            {
                _beamDistance = beamMaxDistance;
            }
            RaycastHit hit;
            Debug.Log("Firing!");


            if (Physics.SphereCast(beamOrigin.position, 0.25f, beamOrigin.forward, out hit, _beamDistance)) { 

                //Skapar bara en sphere så vi vet vart vi träffade.
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                sphere.transform.position = beamOrigin.position + beamOrigin.forward * hit.distance;
                sphere.GetComponent<SphereCollider>().enabled = false;

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
            ServerStatsManager.instance.AddPoint(myTeamID);
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
            if (isLocalPlayer)
            {
                //GetComponentInChildren<MeshRenderer>().material.color = Color.red;
                SpawnManager.instance.Spawn(this.gameObject);

            } else
            {
                //GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
                Debug.Log(transform.name + " HAS DIED");
                //Kill other player
            }
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


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
