using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiplayerPlayerController : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

	// Use this for initialization
	void Start () {
		if (!isLocalPlayer)
        {
            foreach (Behaviour component in componentsToDisable)
            {
                component.enabled = false;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
        {
            return;
        }

        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Translate(0, 0, z);

        MouseLook();

    }

    private void MouseLook()
    {
        Debug.Log("we tried lol");
        yaw += Input.GetAxis("Mouse X");
        pitch -= Input.GetAxis("Mouse Y");

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }

}
