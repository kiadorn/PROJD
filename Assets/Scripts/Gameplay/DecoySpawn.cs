using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoySpawn : MonoBehaviour {

    public GameObject decoy;
    private GameObject newDecoy;
    public float destructionTime = 2f;
    public PlayerController controller;

    public float abilityCooldown = 8f;

    public float cooldown = 0f;

    // Use this for initialization
    void Start () {
        abilityCooldown = 4f;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("f")&& cooldown<=0) {
            cooldown = abilityCooldown;
            newDecoy = Instantiate(decoy) as GameObject;
            Destroy(newDecoy, destructionTime);
            newDecoy.transform.rotation = transform.rotation;
            newDecoy.GetComponent<DecoyBehaviour>().controller = controller;
            newDecoy.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y-0.8f, transform.localPosition.z);

            //newDecoy.animator.SetFloat("Velocity", 1);

            ServerStatsManager.instance.StartDecoyTimer(cooldown);

        }
        if (cooldown>0) {
            cooldown = cooldown - Time.deltaTime; 
        }
    }

}
