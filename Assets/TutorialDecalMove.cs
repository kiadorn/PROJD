using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDecalMove : MonoBehaviour {

    float Angle = 0;
    float Speed = (2 * Mathf.PI) / 15;
    float Radius = 7.5f;
    void Update() {
        Angle += Speed * Time.deltaTime;
        float x = Mathf.Cos(Angle) * Radius;
        float z = Mathf.Sin(Angle) * Radius;

        transform.localPosition = new Vector3(x, 0.5f, z);
    }
}
