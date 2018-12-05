using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private BoolVariable canMove;

    [Header("Speed")]
    [SerializeField] private FloatReference strafeSpeed;
    [SerializeField] private FloatReference backwardSpeed;
    [SerializeField] private FloatReference forwardSpeed;
    [SerializeField] private FloatVariable currentTargetSpeed;

    [Header("Acceleration")]
    [SerializeField] private FloatVariable forwardRate;
    [SerializeField] private FloatVariable strafeRate;
    [SerializeField] private FloatVariable speedUpRate;
    [SerializeField] private FloatVariable slowDownRate;
    [SerializeField] private FloatVariable slowDownLimit;

    [Header("Jumping")]
    [SerializeField] private FloatReference jumpForce;
    [SerializeField] private FloatReference jumpDrag;
    [SerializeField] private BoolVariable isJumping;
    [SerializeField] private BoolVariable inputJump;
    [SerializeField] private GameEvent OnStartJump;
    [SerializeField] private GameEvent OnLandJump;

    [Header("Air Control")]
    [SerializeField] private FloatReference airSpeedUpRate;
    [SerializeField] private BoolVariable airControl;

    [Header("Ground Check")]
    [SerializeField] private FloatReference groundedDrag;
    [SerializeField] private FloatReference groundCheckDistance;
    [SerializeField] private FloatReference shellOffset;
    [SerializeField] private BoolVariable isGrounded;
    [SerializeField] private BoolVariable wasPreviouslyGrounded;
    [SerializeField] private Vector3Variable m_GroundContactNormal;

    [Header("Components")]
    [SerializeField] private CapsuleCollider capsule;
    [SerializeField] private Rigidbody m_Rigidbody;

    public void UpdateHorizontalMovement(Vector2 input) {

        UpdateDesiredTargetSpeed(input);

        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (airControl || isGrounded) && canMove) {
            forwardRate.Value = Mathf.Lerp(forwardRate.Value, input.y, Time.deltaTime * speedUpRate.Value);
            strafeRate.Value = Mathf.Lerp(strafeRate.Value, input.x, Time.deltaTime * speedUpRate.Value);

            Vector3 desiredMove = transform.forward * forwardRate.Value + transform.right * strafeRate.Value;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal.Value).normalized;
            if (!isGrounded) currentTargetSpeed.Value *= airSpeedUpRate;
            desiredMove.x = desiredMove.x * currentTargetSpeed.Value;
            desiredMove.z = desiredMove.z * currentTargetSpeed.Value;
            desiredMove.y = m_Rigidbody.velocity.y;
            desiredMove = Vector3.Lerp(m_Rigidbody.velocity, desiredMove, Time.deltaTime * speedUpRate.Value);

            if (m_Rigidbody.velocity.sqrMagnitude < (currentTargetSpeed.Value * currentTargetSpeed.Value)) {
                m_Rigidbody.velocity = desiredMove; // m_Rigidbody.AddForce(desiredMove, ForceMode.Impulse);
            }
        } else
        {
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, Vector3.zero, Time.deltaTime * slowDownRate.Value);
        }

        if (isGrounded) {
            m_Rigidbody.useGravity = false;
            m_Rigidbody.drag = groundedDrag;
            
        }
        else {
            m_Rigidbody.drag = 0f;
            m_Rigidbody.useGravity = true;
        }
        
    }

    public void UpdateJump() {
        if (inputJump && isGrounded) {
            m_Rigidbody.drag = jumpDrag;
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0f, m_Rigidbody.velocity.z);
            m_Rigidbody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            isJumping.SetValue(true);
        }
        inputJump.SetValue(false);
    }

    private void UpdateDesiredTargetSpeed(Vector2 input) {
        if (input == Vector2.zero) return;
        if (input.x > 0 || input.x < 0) {
            //strafe
            currentTargetSpeed.Value = strafeSpeed.Value;
        }
        if (input.y < 0) {
            //backwards
            currentTargetSpeed.Value = backwardSpeed.Value;
        }
        if (input.y > 0) {
            //forwards
            //handled last as if strafing and moving forward at the same time forwards speed should take precedence
            currentTargetSpeed.Value = forwardSpeed.Value;
        }
    }

    public void GroundCheck() {
        wasPreviouslyGrounded.SetValue(isGrounded);
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capsule.radius * (1.0f - shellOffset), Vector3.down, out hitInfo, 
            ((capsule.height / 2f) - capsule.radius) + groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            isGrounded.SetValue(true);
            m_GroundContactNormal.SetValue(hitInfo.normal);
        }
        else {
            isGrounded.SetValue(false);
            m_GroundContactNormal.SetValue(Vector3.up);
        }

        if (!wasPreviouslyGrounded && isGrounded) {
            if (isJumping)
                isJumping.SetValue(false);
        }
    }

}
