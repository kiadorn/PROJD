using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

public class MaterialSwap : NetworkBehaviour
{
    [Header("References")]
    public PlayerController controller;
    public AudioMixer audioMixer;
    public PostProcessProfile postProcess; 
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
    private Vignette vig;

    public bool visible = true;
    bool previouslyVisible = true;

    void Start()
    {
        //meshr = gameObject.GetComponent<MeshRenderer>();
        mask = 1 << 8;
        postProcess.TryGetSettings<Vignette>(out vig);
    }

    void Update()
    {
        if (!controller.Dead)
        {
            //if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            if (Physics.SphereCast(transform.position, 0.1f, Vector3.down, out hit, 2f, mask, QueryTriggerInteraction.Ignore))
            {
                
                if (hit.transform.GetComponent<MaterialAffiliation>().matAff.ToString() == controller.myTeam.ToString())
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
        if (visible != previouslyVisible)
        {

            GetComponent<PlayerController>().PlayNewAreaSound(visible);
            previouslyVisible = visible;
            if (visible)
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
        visible = true;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = 0;
        emission.rateOverDistance = 0;
        /*firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, controller.myAsset.maskColor, Time.deltaTime * speedMultiplier);*/

        float tpValue = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 0, Time.deltaTime * speedMultiplier);
        float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Alpha"), 1, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.material.SetFloat("_Timer", tpValue);
        firstPersonModel.material.SetFloat("_Alpha", fpValue);

        vig.intensity.value = Mathf.Lerp(vig.intensity.value, 0, Time.deltaTime);
    }

    public void TurnVisibleInstant()
    {
        visible = true;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = 0;
        emission.rateOverDistance = 0;

        thirdPersonModel.material.SetFloat("_Timer", 0);
        thirdPersonMask.material.SetFloat("_Timer", 0);
        firstPersonModel.material.SetFloat("_Alpha", 1);
        //firstPersonModel.material.color = controller.myAsset.bodyColor;
        CmdTurnVisibleInstant();
        vig.intensity.value = 0;
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
            visible = true;
            ParticleSystem.EmissionModule emission = invisibleTrail.emission;
            emission.rateOverTime = 0;
            emission.rateOverDistance = 0;
            thirdPersonModel.material.color = controller.myAsset.bodyColor;
            thirdPersonMask.material.color = controller.myAsset.maskColor;
        }
    }

    private void TurnInvisible()
    {
        visible = false;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = emissionRateWhenInvisible;
        emission.rateOverDistance = emissionRateOverDistanceWhenInvisible;
        //firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, firstPersonTransperancy), Time.deltaTime * speedMultiplier);
        //thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, 0), Time.deltaTime * speedMultiplier);
        //thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, ChangeAlphaTo(controller.myAsset.maskColor, 0), Time.deltaTime * speedMultiplier);

        float tpValue = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 1, Time.deltaTime * speedMultiplier);
        float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Alpha"), firstPersonTransperancy, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.material.SetFloat("_Timer", tpValue);
        firstPersonModel.material.SetFloat("_Alpha", fpValue);

        vig.intensity.value = Mathf.Lerp(vig.intensity.value, 0.6f, Time.deltaTime);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }
}

