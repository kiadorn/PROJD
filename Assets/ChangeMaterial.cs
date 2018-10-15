using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
        {
            Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
            var pixelUV = hit.textureCoord;
            pixelUV.x *= textureMap.width;
            pixelUV.y *= textureMap.height;

            print(gameObject.name + ": " + "x=" + pixelUV.x + ",y=" + pixelUV.y + " " + textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y));

            if(textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r     < 1)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.blue;
            }
            else gameObject.GetComponent<Renderer>().material.color = Color.green;

        }

    }
}
