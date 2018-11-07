using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class DecoyBehaviour : MonoBehaviour {

    public float movementSpeed = 0.5f;

    public bool canMove = true;

    public Animator  animator;


    //from Material Swap

    public bool deathController=false;
    public PlayerController controller;
    public AudioMixer audioMixer;
    public SkinnedMeshRenderer thirdPersonModel;
    public MeshRenderer thirdPersonMask;
    //public SkinnedMeshRenderer firstPersonModel;

    //public ParticleSystem invisibleTrail;

    public float speedMultiplier;
    //public float firstPersonTransperancy = 0.3f;

    //[Range(-1, 1)] float fade;


    RaycastHit hit;
    //MeshRenderer meshr;
    int mask;
    //bool fadeOut = false;
    //bool fadeIn = false;

    bool invisible = true;
    bool previousInvisible = true;


    // Use this for initialization
    void Start () {
        Color c1 = thirdPersonModel.material.color; //this is a problem, fix. It changes main characters transparensy to 0
        thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, 0);
        Color c2 = thirdPersonMask.material.color;
        thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, 0);
        mask = 1 << 8;
    }
	
	// Update is called once per frame
	void Update () {
        if(canMove)
            transform.position += transform.forward * movementSpeed;

        if (!deathController)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, mask))
            {
                Texture2D textureMap = (Texture2D)hit.transform.GetComponent<Renderer>().material.mainTexture;
                var pixelUV = hit.textureCoord;
                pixelUV.x *= textureMap.width;
                pixelUV.y *= textureMap.height;

                
                float floorColorValue = (controller.myTeam == PlayerController.Team.White) ? textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).g : 1 - textureMap.GetPixel((int)pixelUV.x, (int)pixelUV.y).g;
                Debug.Log(floorColorValue+" "+ controller.myAsset.colorLimit);
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

   
    public void Death()
    {
        deathController = true;
        canMove = false;
        animator.SetBool("Death",true);
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        StartCoroutine(CharacterFade());
    }

    private IEnumerator CharacterFade()
    {


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
        thirdPersonModel.material.color = Color.Lerp(thirdPersonModel.material.color, ChangeAlphaTo(controller.myAsset.bodyColor, 0), Time.deltaTime * speedMultiplier);
        thirdPersonMask.material.color = Color.Lerp(thirdPersonMask.material.color, ChangeAlphaTo(controller.myAsset.maskColor, 0), Time.deltaTime * speedMultiplier);

    }

    private Color ChangeAlphaTo(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }



}
