using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public float minRot;
    private Quaternion pos1;
    public float maxRot;
    public float speed;
    private Quaternion pos2;
    bool rotateLeft;

	void Start () {
        pos1 = Quaternion.Euler(new Vector3(transform.rotation.x, minRot, transform.rotation.z));
        pos2 = Quaternion.Euler(new Vector3(transform.rotation.x, maxRot, transform.rotation.z));
    }
	
	void Update () {
        if (ServerStatsManager.instance.gameStarted) {
            Rotate();
        }
        	
	}

    void Rotate()
    {
        Quaternion targetRot = (rotateLeft) ? pos1 : pos2;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, speed * Time.deltaTime);

        if (transform.rotation == targetRot)
            rotateLeft = !rotateLeft;
    }
}
