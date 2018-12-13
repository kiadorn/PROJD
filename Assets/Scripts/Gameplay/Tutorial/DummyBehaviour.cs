using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class DummyBehaviour : MonoBehaviour {

    //public float movementSpeed = 0.5f;

    //public bool canMove = true;

    public Animator animator;

    //from Material Swap

    public Material ClearedMaterial;
    TutorialProgress tp;
    public bool deathController = false;
    //public PlayerController controller;
    public AudioMixer audioMixer;
    public SkinnedMeshRenderer thirdPersonModel;
    public MeshRenderer thirdPersonMask;
    //public SkinnedMeshRenderer firstPersonModel;

    //public ParticleSystem invisibleTrail;

    //public float firstPersonTransperancy = 0.3f;

    //[Range(-1, 1)] float fade;


    RaycastHit hit;
    //MeshRenderer meshr;
    int mask;
    //bool fadeOut = false;
    //bool fadeIn = false;

    bool invisible = true;
    bool previousInvisible = true;
    public static int DeathCount = 1;


    // Use this for initialization
    void Start()
    {
        if (GameObject.Find("Tutorial Manager"))
            tp = GameObject.Find("Tutorial Manager").GetComponent<TutorialProgress>();
        //Color c1 = thirdPersonModel.material.color; //this is a problem, fix. It changes main characters transparensy to 0
        //thirdPersonModel.material.color = new Color(c1.r, c1.g, c1.b, 0);
        //Color c2 = thirdPersonMask.material.color;
        //thirdPersonMask.material.color = new Color(c2.r, c2.g, c2.b, 0);
        mask = 1 << 8;
    }

    // Update is called once per frame
    void Update()
    {
       

        if (deathController)
        {
            Death();

        }
        
        


    }

    public void Death()
    {
        deathController = false;
        animator.SetBool("Death", true);
        GetComponent<CapsuleCollider>().enabled = false;
        //StartCoroutine(ResetDummy());
        if (RoundManager.instance)
            RoundManager.instance.AddPoint(1, 100);
        tp.ShootyRoomProgress++;
        print(transform.parent.name);
        if(transform.parent.name == "Cube") {
            transform.parent.GetComponent<MeshRenderer>().material = ClearedMaterial;
        }
    }

    IEnumerator ResetDummy()
    {
        yield return new WaitForSeconds(2);       
        animator.SetBool("Death", false);
        GetComponent<CapsuleCollider>().enabled = true;
        yield return 0;
    }

}
