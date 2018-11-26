using UnityEngine;

public class PlayerInput : MonoBehaviour {

    public BoolVariable pauseInput;
    public BoolVariable inputJump;
    public BoolVariable inputDash;
    public BoolVariable inputDecoy;
    public BoolVariable inputFire1;
    public BoolVariable inputFire2;
    private float inputX;
    private float inputY;
    public FloatVariable mouseX;
    public FloatVariable mouseY;
    [HideInInspector]
    public Vector2 inputAxis;

	void Update () {
        if (!pauseInput)
            CheckForInput();
    }

    public void CheckForInput() {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        inputAxis = new Vector2(inputX, inputY);

        mouseX.SetValue(Input.GetAxis("Mouse X"));
        mouseY.SetValue(Input.GetAxis("Mouse Y"));

        if (Input.GetButtonDown("Jump")) {
            inputJump.SetValue(true);
        }

        if (Input.GetButtonDown("Dash")) {
            inputDash.SetValue(true);
        }

        if (Input.GetButtonDown("Decoy")) {
            inputDecoy.SetValue(true);
        }

        if (Input.GetButton("Fire1")) {
            inputFire1.SetValue(true);
        }

        if (Input.GetButtonDown("Fire2")) {
            inputFire2.SetValue(true);
        }
    }

    public void ResetInput() {
        inputAxis = Vector2.zero;
    }
}
