using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatoPotato : MonoBehaviour {

    float Rotation;
    public float rotationSpeed;

	void Update () {

        Rotation += Time.deltaTime*rotationSpeed;

        transform.rotation = new Quaternion(0, Rotation, 0, 0);


    }
}
