using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class DecoyBehaviour : NetworkBehaviour {

    public float movementSpeed = 0.5f;

    public bool canMove = true;
    public bool dummy = false;

    public Animator  animator;

    public float destructionTime = 3;

    public bool deathController=false;
    public PlayerController controller;
    public AudioMixer audioMixer;
    public SkinnedMeshRenderer thirdPersonModel;
    public MeshRenderer thirdPersonMask;

    public float speedMultiplier;

    public float targetTransparency = 0;

    public GameObject smoke;

    RaycastHit hit;
    int mask;

    bool visible = true;
    bool previouslyVisible = true;

    Coroutine deathTimer;

    Coroutine deathFade;

    bool usingCoolAlpha = false;
    float alphaValue;

    void Start () {
        

        if (dummy)
        {

            thirdPersonModel.material.SetFloat("_Timer", 0);
            thirdPersonMask.material.SetFloat("_Timer", 0);
            thirdPersonModel.material.SetFloat("_Alpha", 1);
            thirdPersonMask.material.SetFloat("_Alpha", 1);
            thirdPersonMask.materials[1].SetFloat("_Timer", 0);
            thirdPersonModel.materials[1].SetFloat("_Timer", 0);
            thirdPersonMask.materials[1].SetFloat("_Alpha", 1);
            thirdPersonModel.materials[1].SetFloat("_Alpha", 1);
        }
        else
        {

            thirdPersonModel.material.SetFloat("_Timer", 1);
            thirdPersonMask.material.SetFloat("_Timer", 1);
            thirdPersonModel.material.SetFloat("_Alpha", 0);
            thirdPersonMask.material.SetFloat("_Alpha", 0);
            thirdPersonMask.materials[1].SetFloat("_Timer", 1);
            thirdPersonModel.materials[1].SetFloat("_Timer", 1);
            thirdPersonMask.materials[1].SetFloat("_Alpha", 0);
            thirdPersonModel.materials[1].SetFloat("_Alpha", 0);
        }
        
        mask = 1 << 8;

        if (dummy == false)
        {
            deathTimer = StartCoroutine(SpawnDeathCountdown());
        }
        

        smoke.SetActive(false);

        if (controller)
        {
            ParticleSystem.ColorOverLifetimeModule col = smoke.GetComponent<ParticleSystem>().colorOverLifetime;
            col.color = controller.myAsset.bodyColor;

        }

    }
	
	void Update () {
        if (!deathController)
        {
            if (!dummy)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
                {
                 
                    if(hit.transform.GetComponent<MaterialAffiliation>().matAff.ToString() == controller.myTeam.ToString())
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
        }
        else
        {
            TurnVisible();
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    public void Death()
    {
        if (deathTimer != null)
            StopCoroutine(deathTimer);
        if (dummy == true)
        {            
            deathFade = StartCoroutine(DeathFade());            
        }

        smoke.SetActive(true);

        deathController = true;
        canMove = false;
        animator.SetBool("DummyDecoy", false);
        animator.SetBool("Death",true);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        SoundManager.instance.PlayDecoyPoof(gameObject);

        StartCoroutine(DeathCountdown());
        StartCoroutine(DeathFade());
    }


    private IEnumerator DeathFade()
    {

        float newAlpha = 0f;


        while(thirdPersonModel.material.GetFloat("_Timer") < 1f)
        {
            newAlpha += speedMultiplier * Time.deltaTime;
            if (newAlpha > 1f)
            {

                newAlpha = 1f;
            }

            thirdPersonModel.material.SetFloat("_Timer", newAlpha);
            thirdPersonMask.material.SetFloat("_Timer", newAlpha);
            thirdPersonModel.material.SetFloat("_Alpha", 1 - newAlpha);
            thirdPersonMask.material.SetFloat("_Alpha", 1 - newAlpha);
            thirdPersonMask.materials[1].SetFloat("_Timer", newAlpha);
            thirdPersonModel.materials[1].SetFloat("_Timer", newAlpha);
            thirdPersonMask.materials[1].SetFloat("_Alpha", 1 - newAlpha);
            thirdPersonModel.materials[1].SetFloat("_Alpha", 1 - newAlpha);

            yield return 0;
        }

        

        yield return 0;
    }

    private IEnumerator DeathCountdown()
    {

        yield return new WaitForSeconds(destructionTime);
        if (dummy == true)
        {
            smoke.SetActive(false);

            if (deathFade != null)
                StopCoroutine(deathFade);
            thirdPersonModel.material.SetFloat("_Timer", 0);
            thirdPersonMask.material.SetFloat("_Timer", 0);
            thirdPersonModel.material.SetFloat("_Alpha", 1);
            thirdPersonMask.material.SetFloat("_Alpha", 1);
            thirdPersonMask.materials[1].SetFloat("_Timer", 0);
            thirdPersonModel.materials[1].SetFloat("_Timer", 0);
            thirdPersonMask.materials[1].SetFloat("_Alpha", 1);
            thirdPersonModel.materials[1].SetFloat("_Alpha", 1);

            deathController = false;
            animator.SetBool("DummyDecoy", true);
            animator.SetBool("Death", false);
            GetComponent<CapsuleCollider>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
        }

        else
        {
            Destroy(gameObject);
        }

        yield return 0;
    }

    private IEnumerator SpawnDeathCountdown()
    {
        yield return new WaitForSeconds(destructionTime);
        Death();
        yield return 0;
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<CapsuleCollider>().isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void CheckIfNewArea()
    {
        if (visible != previouslyVisible)
        {
            //GetComponent<PlayerController>().PlayNewAreaSound(invisible);
            previouslyVisible = visible;
        }
    }

    private void TurnVisible()
    {
        visible = true;

        float value = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 0, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", value);
        thirdPersonMask.material.SetFloat("_Timer", value);
        thirdPersonModel.material.SetFloat("_Alpha", 1 - value);
        thirdPersonMask.material.SetFloat("_Alpha", 1 - value);
        thirdPersonMask.materials[1].SetFloat("_Timer", value);
        thirdPersonModel.materials[1].SetFloat("_Timer", value);
        thirdPersonMask.materials[1].SetFloat("_Alpha", 1 - value);
        thirdPersonModel.materials[1].SetFloat("_Alpha", 1 - value);
    }

    private void TurnInvisible()
    {

        visible = false;

        float value = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 1f, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", value);
        thirdPersonMask.material.SetFloat("_Timer", value);
        thirdPersonModel.material.SetFloat("_Alpha", 1.3f - value);
        thirdPersonMask.material.SetFloat("_Alpha", 1.3f - value);
        thirdPersonMask.materials[1].SetFloat("_Timer", value);
        thirdPersonModel.materials[1].SetFloat("_Timer", value);
        thirdPersonMask.materials[1].SetFloat("_Alpha", 1.3f - value);
        thirdPersonModel.materials[1].SetFloat("_Alpha", 1.3f - value);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }



}
