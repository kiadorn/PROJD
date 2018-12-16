using UnityEngine;

public class IntroCameraRotation : MonoBehaviour {
    [SerializeField]
    private Camera _cam;
    private bool _rotating = false;
    public float RotationSpeed;

	
	// Update is called once per frame
	private void Update () {
        if (_rotating) {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        }
	}

    public void ChangeIntroCamera(int newDepth, bool cameraStatus) {
        _rotating = cameraStatus;
        _cam.depth = newDepth;
    }

    public void TurnOffIntroCamera() {
        _cam.gameObject.SetActive(false);
    }
}
