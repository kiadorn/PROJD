using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour {

    public float speedMultiplier;
    [Range(-1, 1)] float fade;

    MeshRenderer meshr;
    bool fadeOut = false;
    bool fadeIn = false;

	// Use this for initialization
	void Start () {
        meshr = gameObject.GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        Ray ray = new Ray(transform.position, new Vector3(0, -100, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
            var pixelUV = hit.textureCoord;
            pixelUV.x *= textureMap.width;
            pixelUV.y *= textureMap.height;

            print("x=" + pixelUV.x + ",y=" + pixelUV.y + " " + textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y));
        }

        if (Input.GetKey(KeyCode.Alpha1)){
            fadeIn = true;
            fadeOut = false;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            fadeIn = false;
            fadeOut = false;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            fadeOut = true;
            fadeIn = false;
        }
        // om fadeIn är sann, minska timer, om fadeOut är sann, öka timern, clampa mellan -1 och 1. 
        
        if(fadeIn == true)
        {
            fade += Time.deltaTime * speedMultiplier;
            float output = Mathf.Clamp(fade, -1, 1);
            meshr.material.SetFloat("Vector1_6C82E8EC", output);
        }
        else if(fadeOut == true)
        {
            fade -= Time.deltaTime * speedMultiplier;
            float output = Mathf.Clamp(fade, -1, 1);
            meshr.material.SetFloat("Vector1_6C82E8EC", output);
        }


        /*
        if (fadeIn == true)
        {
            if (timer > -1)
            {
                timer -= Time.deltaTime*speedMultiplier;
                meshr.material.SetFloat("Vector1_6C82E8EC", timer);
            }
            else fadeIn = false;
        }

        if (fadeOut == true)
        {
            if (timer < 1)
            {
                timer += Time.deltaTime*speedMultiplier;
                meshr.material.SetFloat("Vector1_6C82E8EC", timer);
            }
            else fadeOut = false;
        }
        */
        //("Vector1_6C82E8EC")
    }
}
