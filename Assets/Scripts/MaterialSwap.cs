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
    public SkinnedMeshRenderer firstPersonModelTransparent;
    [Header("Particle System")]
    public ParticleSystem invisibleTrail;
    public float emissionRateWhenInvisible = 0.5f;
    public float emissionRateOverDistanceWhenInvisible = 20f;
    private ParticleSystem.Burst originalBurst;
    [Header("Fade Settings")]
    public float speedMultiplier;
    //public float firstPersonTransparency = 0.3f;
    public float firstPersonTransparency = 1f;
    public float vignetteIntensityWhenInvisible = 0.6f;

    private RaycastHit hit;
    private int mask;
    private Vignette vig;

    public bool isVisible = true;
    bool previouslyVisible = true;

    void Start()
    {
        mask = 1 << 8;
        postProcess.TryGetSettings<Vignette>(out vig);
    }

    void Update()
    {
        if (!controller.Dead)
        {
            //if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            if (Physics.SphereCast(transform.position, 0.1f, Vector3.down, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.GetComponent<MaterialAffiliation>().matAff == controller.myTeam)
                {
                    TurnVisibleState(false);
                }
                else
                {
                    TurnVisibleState(true);
                }
            }

            CheckIfNewArea();
        }
        else
        {
            TurnVisibleState(true);
        }
    }

    private void CheckIfNewArea()
    {
        if (isVisible != previouslyVisible)
        {
            previouslyVisible = isVisible;

            controller.PlayNewAreaSound(isVisible);

            ParticleSystem.EmissionModule emission = invisibleTrail.emission;
            emission.rateOverTime = isVisible ? 0 : emissionRateWhenInvisible;
            emission.rateOverDistance = isVisible ? 0 : emissionRateOverDistanceWhenInvisible;

            if (isVisible)
            {
                audioMixer.FindSnapshot("Other Color").TransitionTo(0.5f);
            }
            else
            {
                audioMixer.FindSnapshot("Own Color").TransitionTo(0.5f);
            }
        }
    }

    private void TurnVisibleState(bool turnVisible) 
    {
        isVisible = turnVisible;

        float tpValue = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), turnVisible ? 0 : 1, Time.deltaTime * speedMultiplier);
        //float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Alpha"), turnVisible ? 1 : firstPersonTransparency, Time.deltaTime * speedMultiplier);
        float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Timer"), turnVisible ? 0 : 1, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.materials[1].SetFloat("_Timer", tpValue);
        thirdPersonModel.materials[1].SetFloat("_Timer", tpValue);
        //firstPersonModel.material.SetFloat("_Alpha", fpValue);
        firstPersonModel.material.SetFloat("_Timer", fpValue);

        vig.intensity.value = Mathf.Lerp(vig.intensity.value, turnVisible ? 0 : vignetteIntensityWhenInvisible, Time.deltaTime * speedMultiplier);
    }

    public void TurnVisibleInstant()
    {
        isVisible = true;

        thirdPersonModel.material.SetFloat("_Timer", 0);
        thirdPersonMask.material.SetFloat("_Timer", 0);
        thirdPersonMask.materials[1].SetFloat("_Timer", 0);
        thirdPersonModel.materials[1].SetFloat("_Timer", 0);
        //firstPersonModel.material.SetFloat("_Alpha", 1);
        firstPersonModel.material.SetFloat("_Timer", 0);

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
            isVisible = true;
            ParticleSystem.EmissionModule emission = invisibleTrail.emission;
            emission.rateOverTime = 0;
            emission.rateOverDistance = 0;

            thirdPersonModel.material.SetFloat("_Timer", 0);
            thirdPersonMask.material.SetFloat("_Timer", 0);
            thirdPersonMask.materials[1].SetFloat("_Timer", 0);
            thirdPersonModel.materials[1].SetFloat("_Timer", 0);
            //firstPersonModel.material.SetFloat("_Alpha", 1);
            firstPersonModel.material.SetFloat("_Timer", 0);

        }
    }

    private void TurnVisible() {
        isVisible = true;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = 0;
        emission.rateOverDistance = 0;

        float tpValue = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 0, Time.deltaTime * speedMultiplier);
        //float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Alpha"), 1, Time.deltaTime * speedMultiplier);
        float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Timer"), 0, Time.deltaTime * speedMultiplier);


        thirdPersonModel.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.material.SetFloat("_Timer", tpValue);
        //firstPersonModel.material.SetFloat("_Alpha", fpValue);
        firstPersonModel.material.SetFloat("_Timer", fpValue);


        vig.intensity.value = Mathf.Lerp(vig.intensity.value, 0, Time.deltaTime * speedMultiplier);
    }

    private void TurnInvisible()
    {
        isVisible = false;
        ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        emission.rateOverTime = emissionRateWhenInvisible;
        emission.rateOverDistance = emissionRateOverDistanceWhenInvisible;

        float tpValue = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 1, Time.deltaTime * speedMultiplier);
        //float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Alpha"), firstPersonTransparency, Time.deltaTime * speedMultiplier);
        float fpValue = Mathf.Lerp(firstPersonModel.material.GetFloat("_Timer"), firstPersonTransparency, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.material.SetFloat("_Timer", tpValue);
        thirdPersonMask.materials[1].SetFloat("_Timer", tpValue);
        thirdPersonModel.materials[1].SetFloat("_Timer", tpValue);
        //firstPersonModel.material.SetFloat("_Alpha", fpValue);
        firstPersonModel.material.SetFloat("_Timer", fpValue);

        vig.intensity.value = Mathf.Lerp(vig.intensity.value, 0.55f, Time.deltaTime * speedMultiplier);
    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }
}

