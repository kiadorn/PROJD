using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwap : MonoBehaviour {

    public Team team;
    public enum Team
    {
        light,
        dark
    }

    public float speedMultiplier;
    [Range(-1, 1)] float fade;

    RaycastHit hit;
    MeshRenderer meshr;
    bool fadeOut = false;
    bool fadeIn = false;

	// Use this for initialization
	void Start () {
        meshr = gameObject.GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {


        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2, 8))
        {
            Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
            var pixelUV = hit.textureCoord;
            pixelUV.x *= textureMap.width;
            pixelUV.y *= textureMap.height;

            print(gameObject.name + ": " + "x=" + pixelUV.x + ",y=" + pixelUV.y + " " + textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y));

            if (team == Team.light)
            {
                if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r > 0)
                {
                    gameObject.GetComponentInChildren<Renderer>().materials[0].color = new Color(0, 0, 0, 0);
                }
                else
                {
                    gameObject.GetComponentInChildren<Renderer>().materials[0].color = new Color(1, 1, 1, 1);
                }
            }

            if (team == Team.dark)
            {
                if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r < 0.8)
                {
                    gameObject.GetComponentInChildren<Renderer>().materials[0].color = new Color(0, 0, 0, 0);
                }
                else
                {
                    gameObject.GetComponentInChildren<Renderer>().materials[0].color = new Color(0, 0, 0, 1);
                }
            }
                // check if color is different from previous check, fade for different teams, if you're light team, fade away if ground is light enough, if you're dark team, fade if ground is dark enough.
                /*
                            if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r < 1)
                            {
                                gameObject.GetComponent<Renderer>().material.color = Color.blue;
                            }
                            else gameObject.GetComponent<Renderer>().material.color = Color.green;
                            */
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
