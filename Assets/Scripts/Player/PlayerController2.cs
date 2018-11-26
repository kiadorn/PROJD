using UnityEngine;

public class PlayerController2 : MonoBehaviour {

    public PlayerInput input;
    public PlayerMovement movement;
    public PlayerCameraRotate cameraRotate;
    public PlayerDash dash;

	void Update () {
        cameraRotate.LookRotation(transform);
        movement.UpdateJump();
    }

    void FixedUpdate() {
        movement.GroundCheck();
        movement.UpdateHorizontalMovement(input.inputAxis);
    }



}
