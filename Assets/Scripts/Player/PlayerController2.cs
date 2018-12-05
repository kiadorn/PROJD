using UnityEngine;

public class PlayerController2 : MonoBehaviour {

    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerCameraRotate cameraRotate;
    [SerializeField] private PlayerDash dash;

	void Update () {
        cameraRotate.LookRotation(transform);
        movement.UpdateJump();
        dash.UpdateDash(input.inputAxis);
    }

    void FixedUpdate() {
        movement.GroundCheck();
        movement.UpdateHorizontalMovement(input.inputAxis);
    }



}
