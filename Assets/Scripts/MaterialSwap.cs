using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MaterialSwap : MonoBehaviour {

    public AudioMixer audioMixer;
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
    int mask;
    bool fadeOut = false;
    bool fadeIn = false;

    bool invisible = true;
    bool previousInvisible = true;

	// Use this for initialization
	void Start () {
        meshr = gameObject.GetComponent<MeshRenderer>();
        mask = 1 << 8;
    }
	
	// Update is called once per frame
	void Update () {

        if (!GetComponent<PlayerController>().Dead)
        {


            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            {
                Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
                var pixelUV = hit.textureCoord;
                pixelUV.x *= textureMap.width;
                pixelUV.y *= textureMap.height;

                //print(gameObject.name + ": " + "x=" + pixelUV.x + ",y=" + pixelUV.y + " " + textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y));

                if (team == Team.light)
                {
                    if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r > 0.7f)
                    {
                        invisible = true;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = new Color(0, 0, 0, 0);
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = new Color(0, 0, 0, 0);
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = new Color(0, 0, 0, 0);
                        audioMixer.FindSnapshot("Own Color").TransitionTo(0.5f);
                    }
                    else
                    {
                        invisible = false;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.white;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = Color.black;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = Color.white;
                        audioMixer.FindSnapshot("Other Color").TransitionTo(0.5f);
                    }
                }

                if (team == Team.dark)
                {
                    if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r < 0.3f)
                    {
                        invisible = true;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = new Color(0, 0, 0, 0);
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = new Color(0, 0, 0, 0);
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = new Color(0, 0, 0, 0);
                        audioMixer.FindSnapshot("Own Color").TransitionTo(0.5f);
                    }
                    else
                    {
                        invisible = false;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.black;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = Color.white;
                        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = Color.black;
                        audioMixer.FindSnapshot("Other Color").TransitionTo(0.5f);
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
        } 
        else
        {
            if (team == Team.dark)
            {
                invisible = false;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.black;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = Color.white;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = Color.black;
            }
            else if (team == Team.light)
            {
                invisible = false;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.white;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[1].color = Color.black;
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials[2].color = Color.white;
            }
        }

        //if (Input.GetKey(KeyCode.Alpha1)){
        //    fadeIn = true;
        //    fadeOut = false;
        //}
        //if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    fadeIn = false;
        //    fadeOut = false;
        //}
        //if (Input.GetKey(KeyCode.Alpha3))
        //{
        //    fadeOut = true;
        //    fadeIn = false;
        //}
        
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

        CheckIfNewArea();

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

    private void CheckIfNewArea() {
        if(invisible != previousInvisible) {
            GetComponent<PlayerController>().PlayNewAreaSound(invisible);
            previousInvisible = invisible;
        }
    }
}
