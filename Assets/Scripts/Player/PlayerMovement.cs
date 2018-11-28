using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public BoolVariable canMove;

    [Header("Speed")]
    public FloatReference strafeSpeed;
    public FloatReference backwardSpeed;
    public FloatReference forwardSpeed;
    public FloatReference currentTargetSpeed;

    [Header("Acceleration")]
    public FloatVariable forwardRate;
    public FloatVariable strafeRate;
    public FloatVariable speedUpRate;
    public FloatVariable slowDownRate;
    public FloatVariable slowDownLimit;

    [Header("Jumping")]
    public FloatReference jumpForce;
    public FloatReference jumpDrag;
    public BoolVariable isJumping;
    public BoolVariable inputJump;

    [Header("Air Control")]
    public FloatReference airSpeedUpRate;
    public BoolVariable airControl;

    [Header("Ground Check")]
    public FloatReference groundedDrag;
    public FloatReference groundCheckDistance;
    public FloatReference shellOffset;
    public BoolVariable isGrounded;
    public BoolVariable wasPreviouslyGrounded;
    public Vector3Variable m_GroundContactNormal;

    [Header("Components")]
    public CapsuleCollider capsule;
    public Rigidbody m_Rigidbody;

    public void UpdateHorizontalMovement(Vector2 input) {

        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (airControl || isGrounded) && canMove) {
            forwardRate.Value = Mathf.Lerp(forwardRate.Value, input.y, Time.deltaTime * speedUpRate.Value);
            strafeRate.Value = Mathf.Lerp(strafeRate.Value, input.x, Time.deltaTime * speedUpRate.Value);

            Vector3 desiredMove = transform.forward * forwardRate.Value + transform.right * strafeRate.Value;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal.Value).normalized;
            if (!isGrounded) desiredMove *= airSpeedUpRate;
            desiredMove.x = desiredMove.x * currentTargetSpeed;
            desiredMove.z = desiredMove.z * currentTargetSpeed;

            if (m_Rigidbody.velocity.sqrMagnitude < (currentTargetSpeed * currentTargetSpeed)) {
                m_Rigidbody.AddForce(desiredMove, ForceMode.Impulse);
            }
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
            currentTargetSpeed = strafeSpeed;
        }
        if (input.y < 0) {
            //backwards
            currentTargetSpeed = backwardSpeed;
        }
        if (input.y > 0) {
            //forwards
            //handled last as if strafing and moving forward at the same time forwards speed should take precedence
            currentTargetSpeed = forwardSpeed;
        }
    }

    public void GroundCheck() {
        wasPreviouslyGrounded = isGrounded;
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
