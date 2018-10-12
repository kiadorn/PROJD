using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFade : MonoBehaviour {
    public float fadeSpeed = 0.2f;
    // Use this for initialization

    public Material standardMaterial;
    public Material transparentMaterial;


    void Start () {
        ChangeTransparency();
        

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
            ChangeTransparency();
    }

    private void ChangeTransparency()
    {
        if (this.gameObject.GetComponent<MeshRenderer>().material == standardMaterial)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        else if (this.gameObject.GetComponent<MeshRenderer>().material == transparentMaterial)
        {
            this.gameObject.GetComponent<MeshRenderer>().material = standardMaterial;
        }
        //Debug.Log(this.gameObject.GetComponent<MeshRenderer>().material);
    }
}
