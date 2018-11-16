using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateFade : MonoBehaviour {

    public AnimationCurve curve;
    MeshRenderer renderer;
    Material gateMat;
    float alpha = 1;

	void Start () {
        renderer = GetComponent<MeshRenderer>();
        gateMat = renderer.material;
        print(renderer);
        print(gateMat);

	}
	
	void Update () {
        if(Input.GetKeyDown(KeyCode.L)) {
            alpha = 1;
        }

        gateMat.SetFloat("Vector1_65A983A5", curve.Evaluate(Time.time));

    }
}
