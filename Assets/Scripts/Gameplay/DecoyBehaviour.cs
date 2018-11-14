using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class DecoyBehaviour : MonoBehaviour {

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

    RaycastHit hit;
    int mask;

    bool invisible = true;
    bool previousInvisible = true;

    Coroutine deathTimer;

    void Start () {
        Color c1 = thirdPersonModel.material.color; //this is a problem, fix. It changes main characters transparensy to 0
        thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, 0);
        Color c2 = thirdPersonMask.material.color;
        thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, 0);
        mask = 1 << 8;

        if (dummy == false)
        {
            deathTimer = StartCoroutine(SpawnDeathCountdown());
        }
    }
	
	void Update () {
        if(canMove)
            transform.position += transform.forward * movementSpeed * Time.deltaTime;

        if (!deathController)
        {

            if (!dummy)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
                {
                    Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
                    var pixelUV = hit.textureCoord;
                    pixelUV.x *= textureMap.width;
                    pixelUV.y *= textureMap.height;


                    float floorColorValue = (controller.myTeam == PlayerController.Team.White) ? textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).g : 1 - textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).g;
                    Debug.Log(floorColorValue + " " + controller.myAsset.colorLimit);
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
        }
        else
        {
            TurnVisible();
            
        }


    }

   
    public void Death()
    {
        if (deathTimer != null)
            StopCoroutine(deathTimer);


        deathController = true;
        canMove = false;
        animator.SetBool("DummyDecoy", false);
        animator.SetBool("Death",true);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;

        StartCoroutine(DeathCountdown());

    }

    private IEnumerator DeathCountdown()
    {

        yield return new WaitForSeconds(destructionTime);
        if (dummy == true)
        {
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
       // ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        //emission.enabled = false;
        //emission.rateOverTime = 0;
        //firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, controller.myAsset.bodyColor, Time.deltaTime * speedMultiplier);
        thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, controller.myAsset.maskColor, Time.deltaTime * speedMultiplier);
    }

    private void TurnInvisible()
    {

        invisible = false;
        //ParticleSystem.EmissionModule emission = invisibleTrail.emission;
        //emission.enabled = true;
        //emission.rateOverTime = 0.5f;
        //firstPersonModel.material.color = Color.Lerp(firstPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, firstPersonTransperancy), Time.deltaTime * speedMultiplier);
        thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, targetTransparency), Time.deltaTime * speedMultiplier);
        thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, ChangeAlphaTo(controller.myAsset.maskColor, targetTransparency), Time.deltaTime * speedMultiplier);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }



}
