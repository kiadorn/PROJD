using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class MaterialSwap : NetworkBehaviour
{
    [Header("Component References")]
    public PlayerController controller;
    public AudioMixer audioMixer;
    [Header("Models")]
    public SkinnedMeshRenderer thirdPersonModel;
    public MeshRenderer thirdPersonMask;
    public SkinnedMeshRenderer firstPersonModel;
    [Header("Particle System")]
    public ParticleSystem invisibleTrail;
    public float emissionRateWhenInvisible = 0.5f;
    public float emissionRateOverDistanceWhenInvisible = 20f;
    private ParticleSystem.Burst originalBurst;
    [Header("Fade Settings")]
    public float speedMultiplier;
    public float firstPersonTransperancy = 0.3f;


    private RaycastHit hit;
    private int mask;

    bool invisible = true;
    bool previousInvisible = true;

    void Start()
    {
        //meshr = gameObject.GetComponent<MeshRenderer>();
        mask = 1 << 8;
    }

    void Update()
    {
        if (!controller.Dead)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            //if (Physics.SphereCast(transform.position, 0.1f, Vector3.down, out hit, 2f, mask, QueryTriggerInteraction.Ignore))
            {
                Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
                var pixelUV = hit.textureCoord;
                try
                {
                    pixelUV.x *= textureMap.width;
                    pixelUV.y *= textureMap.height;
                } 
                catch (NullReferenceException ex)
                {
                    print("Kan inte hitta TextureMap hos " + hit.collider.name);
                    return;
                }

                float floorColorValue = (controller.myTeam == PlayerController.Team.White) ? textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).g : 1 - textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).g;

                if (floorColorValue > controller.myAsset.colorLimit)
                {
                    TurnInvisible();
                }
                else
                {
                    TurnVisible();
                }
            }

            CheckIfNewArea();
        }
        else
        {
            TurnVisible();
        }
    }

    private void CheckIfNewArea()
    {
        if (invisible != previousInvisible)
        {

            GetComponent<PlayerController>().PlayNewAreaSound(invisible);
            previousInvisible = invisible;
            if (invisible)
            {
                audioMixer.FindSnapshot("Other Color").TransitionTo(0.5f);
            }
            else
            {
                audioMixer.FindSnapshot("Own Color").TransitionTo(0.5f);
            }
        }
        
    }

    private void TurnVisible()
    {
        invisible = true;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = 0;
        emission.rateOverDistance = 0;
        firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, controller.myAsset.maskColor, Time.deltaTime * speedMultiplier);
    }

    public void TurnVisibleInstant()
    {
        invisible = true;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = 0;
        emission.rateOverDistance = 0;
        firstPersonModel.material.color = controller.myAsset.bodyColor;
        CmdTurnVisibleInstant();
    }

    [Command]
    public void CmdTurnVisibleInstant()
    {
        RpcTurnVisibleInstant();
    }

    [ClientRpc]
    public void RpcTurnVisibleInstant()
    {
        if (!isLocalPlayer)
        {
            invisible = true;
            ParticleSystem.EmissionModule emission = invisibleTrail.emission;
            emission.rateOverTime = 0;
            emission.rateOverDistance = 0;
            thirdPersonModel.material.color = controller.myAsset.bodyColor;
            thirdPersonMask.material.color = controller.myAsset.maskColor;
        }
    }

    private void TurnInvisible()
    {
        invisible = false;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = emissionRateWhenInvisible;
        emission.rateOverDistance = emissionRateOverDistanceWhenInvisible;
        firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, firstPersonTransperancy), Time.deltaTime * speedMultiplier);
        thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, 0), Time.deltaTime * speedMultiplier);
        thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, ChangeAlphaTo(controller.myAsset.maskColor, 0), Time.deltaTime * speedMultiplier);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
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