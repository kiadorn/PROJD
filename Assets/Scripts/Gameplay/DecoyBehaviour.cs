using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyBehaviour : MonoBehaviour {

    public float movementSpeed = 0.5f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        transform.position += transform.forward * movementSpeed;
    }

   
    public void Death()
    {
        movementSpeed = 0;
        GetComponent<CapsuleCollider>().enabled = false;
        StartCoroutine(CharacterFade());
    }

    private IEnumerator CharacterFade()
    {


        yield return 0;
    }

}
