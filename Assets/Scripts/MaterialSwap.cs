using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class MaterialSwap : NetworkBehaviour
{

    public PlayerController controller;
    public AudioMixer audioMixer;
    public SkinnedMeshRenderer thirdPersonModel;
    public MeshRenderer thirdPersonMask;
    public SkinnedMeshRenderer firstPersonModel;

    public float speedMultiplier;
    public float firstPersonTransperancy = 0.3f;

    [Range(-1, 1)] float fade;

    
    RaycastHit hit;
    MeshRenderer meshr;
    int mask;
    bool fadeOut = false;
    bool fadeIn = false;

    bool invisible = true;
    bool previousInvisible = true;

    void Start()
    {
        meshr = gameObject.GetComponent<MeshRenderer>();
        mask = 1 << 8;
    }

    void Update()
    {
        if (controller.Dead)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            {
                Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
                var pixelUV = hit.textureCoord;
                pixelUV.x *= textureMap.width;
                pixelUV.y *= textureMap.height;

                if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r > ((controller.myTeam == PlayerController.Team.White) ? controller.myAsset.colorLimit : 1 - controller.myAsset.colorLimit))
                {
                    TurnInvisible();
                }
                else
                {
                    TurnVisible();
                }
            }
            else
            {
                TurnVisible();
            }
            CheckIfNewArea();
        }
    }

    private void CheckIfNewArea()
    {
        if (invisible != previousInvisible)
        {
            GetComponent<PlayerController>().PlayNewAreaSound(invisible);
            previousInvisible = invisible;
        }
    }

    private void TurnVisible()
    {
        invisible = true;
        firstPersonModel.materials[0].color = Color.Lerp(firstPersonModel.materials[0].color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonModel.materials[0].color = Color.Lerp(thirdPersonModel.materials[0].color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonModel.materials[1].color = Color.Lerp(thirdPersonModel.materials[1].color, controller.myAsset.maskColor, Time.deltaTime * speedMultiplier);
    }

    private void TurnInvisible()
    {
        invisible = false;
        firstPersonModel.materials[0].color = Color.Lerp(firstPersonModel.materials[0].color, ChangeAlphaTo(controller.myAsset.bodyColor, firstPersonTransperancy), Time.deltaTime * speedMultiplier);
        thirdPersonModel.materials[0].color = Color.Lerp(thirdPersonModel.materials[0].color, ChangeAlphaTo(controller.myAsset.bodyColor, 0), Time.deltaTime * speedMultiplier);
        thirdPersonModel.materials[1].color = Color.Lerp(thirdPersonModel.materials[1].color, ChangeAlphaTo(controller.myAsset.maskColor, 0), Time.deltaTime * speedMultiplier);
    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.b, color.g, alphaValue);
    }
}

//    if (controller.myTeam == PlayerController.Team.White)
//{
//    if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r > 0.7f)
//    {
//        invisible = true;
//        TurnInvisible();
//        audioMixer.FindSnapshot("Own Color").TransitionTo(0.5f);
//    }
//    else
//    {
//        invisible = false;
//        TurnVisibleWhite();
//        audioMixer.FindSnapshot("Other Color").TransitionTo(0.5f);
//    }
//}

//if (controller.myTeam == PlayerController.Team.Black)
//{
//    if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r < 0.3f)
//    {
//        invisible = true;
//        TurnInvisible();
//        audioMixer.FindSnapshot("Own Color").TransitionTo(0.5f);
//    }
//    else
//    {
//        invisible = false;
//        //TurnVisibleBlack();
//        audioMixer.FindSnapshot("Other Color").TransitionTo(0.5f);
//    }
//        }
//        // check if color is different from previous check, fade for different teams, if you're light team, fade away if ground is light enough, if you're dark team, fade if ground is dark enough.
//        /*
//                    if (textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).r < 1)
//                    {
//                        gameObject.GetComponent<Renderer>().material.color = Color.blue;
//                    }
//                    else gameObject.GetComponent<Renderer>().material.color = Color.green;
//                    */
//    }
//} 
//else
//{
//    if (team == Team.dark)
//    {
//        invisible = false;
//        TurnVisibleBlack();
//    }
//    else if (team == Team.light)
//    {
//        invisible = false;
//        TurnVisibleWhite();
//    }
//}

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

//if (fadeIn == true)
//{
//    fade += Time.deltaTime * speedMultiplier;
//    float output = Mathf.Clamp(fade, -1, 1);
//    meshr.material.SetFloat("Vector1_6C82E8EC", output);
//}
//else if (fadeOut == true)
//{
//    fade -= Time.deltaTime * speedMultiplier;
//    float output = Mathf.Clamp(fade, -1, 1);
//    meshr.material.SetFloat("Vector1_6C82E8EC", output);
//}

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