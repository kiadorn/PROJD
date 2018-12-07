using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class DecoyBehaviour : NetworkBehaviour {

    public float movementSpeed = 0.5f;

    public bool canMove = true;
    public bool dummy = false;

    public Animator  animator;

    public float destructionTime = 3;

    public bool deathController=false;
    public PlayerController controller;
    public AudioMixer audioMixer;
    public SkinnedMeshRenderer bodyModel;
    public SkinnedMeshRenderer bodyModelTransparent;
    public MeshRenderer maskModel;

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

            bodyModel.material.SetFloat("_Timer", 0);
            maskModel.material.SetFloat("_Timer", 0);
            //bodyModel.material.SetFloat("_Alpha", 1);
            //maskModel.material.SetFloat("_Alpha", 1);
            maskModel.materials[1].SetFloat("_Timer", 0);
            bodyModel.materials[1].SetFloat("_Timer", 0);
            //maskModel.materials[1].SetFloat("_Alpha", 1);
            //bodyModel.materials[1].SetFloat("_Alpha", 1);
        }
        else
        {

            bodyModel.material.SetFloat("_Timer", 1);
            maskModel.material.SetFloat("_Timer", 1);
            //bodyModel.material.SetFloat("_Alpha", 0);
            //maskModel.material.SetFloat("_Alpha", 0);
            maskModel.materials[1].SetFloat("_Timer", 1);
            bodyModel.materials[1].SetFloat("_Timer", 1);
            //maskModel.materials[1].SetFloat("_Alpha", 0);
            //bodyModel.materials[1].SetFloat("_Alpha", 0);
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
            col.color = controller.myAsset.BodyColor;

        }

    }
	
	void Update () {
        if (!deathController)
        {
            if (!dummy)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
                {
                 
                    if(hit.transform.GetComponent<MaterialAffiliation>().matAff == controller.myTeam)
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


        while(bodyModel.material.GetFloat("_Timer") < 1f)
        {
            newAlpha += speedMultiplier * Time.deltaTime;
            if (newAlpha > 1f)
            {

                newAlpha = 1f;
            }

            bodyModel.material.SetFloat("_Timer", newAlpha);
            maskModel.material.SetFloat("_Timer", newAlpha);
            //bodyModel.material.SetFloat("_Alpha", 1 - newAlpha);
            //maskModel.material.SetFloat("_Alpha", 1 - newAlpha);
            maskModel.materials[1].SetFloat("_Timer", newAlpha);
            bodyModel.materials[1].SetFloat("_Timer", newAlpha);
            //maskModel.materials[1].SetFloat("_Alpha", 1 - newAlpha);
            //bodyModel.materials[1].SetFloat("_Alpha", 1 - newAlpha);

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
            bodyModel.material.SetFloat("_Timer", 0);
            maskModel.material.SetFloat("_Timer", 0);
            //bodyModel.material.SetFloat("_Alpha", 1);
            //maskModel.material.SetFloat("_Alpha", 1);
            maskModel.materials[1].SetFloat("_Timer", 0);
            bodyModel.materials[1].SetFloat("_Timer", 0);
            //maskModel.materials[1].SetFloat("_Alpha", 1);
            //bodyModel.materials[1].SetFloat("_Alpha", 1);

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

        float value = Mathf.Lerp(bodyModel.material.GetFloat("_Timer"), 0, Time.deltaTime * speedMultiplier);

        bodyModel.material.SetFloat("_Timer", value);
        maskModel.material.SetFloat("_Timer", value);
        //bodyModel.material.SetFloat("_Alpha", 1 - value);
        //maskModel.material.SetFloat("_Alpha", 1 - value);
        maskModel.materials[1].SetFloat("_Timer", value);
        bodyModel.materials[1].SetFloat("_Timer", value);
        //maskModel.materials[1].SetFloat("_Alpha", 1 - value);
        //bodyModel.materials[1].SetFloat("_Alpha", 1 - value);
    }

    private void TurnInvisible()
    {

        visible = false;

        float value = Mathf.Lerp(bodyModel.material.GetFloat("_Timer"), 1f, Time.deltaTime * speedMultiplier);

        bodyModel.material.SetFloat("_Timer", value);
        maskModel.material.SetFloat("_Timer", value);
        //bodyModel.material.SetFloat("_Alpha", 1.3f - value);
        //maskModel.material.SetFloat("_Alpha", 1.3f - value);
        maskModel.materials[1].SetFloat("_Timer", value);
        bodyModel.materials[1].SetFloat("_Timer", value);
        //maskModel.materials[1].SetFloat("_Alpha", 1.3f - value);
        //bodyModel.materials[1].SetFloat("_Alpha", 1.3f - value);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }



}
