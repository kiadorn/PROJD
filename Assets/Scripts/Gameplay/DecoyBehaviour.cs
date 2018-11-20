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

    bool invisible = true;
    bool previousInvisible = true;

    Coroutine deathTimer;

    Coroutine deathFade;

    //Color c1;
    //Color c2;

    void Start () {

        if (dummy)
        {
            //c1 = thirdPersonModel.material.color;
            //thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, 1);
            //c2 = thirdPersonMask.material.color;
            //thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, 1);

            thirdPersonModel.material.SetFloat("_Timer", -1);
            thirdPersonMask.material.SetFloat("_Timer", -1);
        }
        else
        {
            //c1 = thirdPersonModel.material.color;
            //thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, 0);
            //c2 = thirdPersonMask.material.color;
            //thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, 0);

            thirdPersonModel.material.SetFloat("_Timer", 1);
            thirdPersonMask.material.SetFloat("_Timer", 1);

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


        

        if (canMove)
            transform.position += transform.forward * movementSpeed * Time.deltaTime;

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

    

    public void Death()
    {
        print("I tried so hard");
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

        StartCoroutine(DeathCountdown());
        StartCoroutine(DeathFade());
    }


    private IEnumerator DeathFade()
    {

        //float newAlpha = 1f;
        float newAlpha = -1f;

        //c1 = thirdPersonModel.material.color;
       // c2 = thirdPersonMask.material.color;

        //while (thirdPersonModel.material.color.a>0)
        while(thirdPersonModel.material.GetFloat("_Timer") < 1f)
        {
            newAlpha += 1f * Time.deltaTime;
            //newAlpha -= 1f*Time.deltaTime;
            //if (newAlpha < 0)
            if (newAlpha > 1f)
            {
                //newAlpha = 0;
                newAlpha = 1f;
            }

            thirdPersonModel.material.SetFloat("_Timer", newAlpha);
            thirdPersonMask.material.SetFloat("_Timer", newAlpha);
            //thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, newAlpha);     
            //thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, newAlpha);

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

            // thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, 1);
            // thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, 1);

            thirdPersonModel.material.SetFloat("_Timer", -1);
            thirdPersonMask.material.SetFloat("_Timer", -1);

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
        if (invisible != previousInvisible)
        {
            //GetComponent<PlayerController>().PlayNewAreaSound(invisible);
            previousInvisible = invisible;
        }
    }

    private void TurnVisible()
    {
        invisible = true;
        //ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        //emission.enabled = false;
        //emission.rateOverTime = 0;
        //firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        //thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        //thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, controller.myAsset.maskColor, Time.deltaTime * speedMultiplier);

        float value = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), -1f, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", value);
        thirdPersonMask.material.SetFloat("_Timer", value);
    }

    private void TurnInvisible()
    {

        invisible = false;
        //ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        //emission.enabled = true;
        //emission.rateOverTime = 0.5f;
        //firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, firstPersonTransperancy), Time.deltaTime * speedMultiplier);
        //thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, targetTransparency), Time.deltaTime * speedMultiplier);
        //thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, ChangeAlphaTo(controller.myAsset.maskColor, targetTransparency), Time.deltaTime * speedMultiplier);

        float value = Mathf.Lerp(thirdPersonModel.material.GetFloat("_Timer"), 1f, Time.deltaTime * speedMultiplier);

        thirdPersonModel.material.SetFloat("_Timer", value);
        thirdPersonMask.material.SetFloat("_Timer", value);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }



}
