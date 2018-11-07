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
            CreateDecoy(transform.rotation, transform.position);
        }
        if (cooldown>0) {
            cooldown = cooldown - Time.deltaTime; 
        }
    }

    private void CreateDecoy(Quaternion decoyRotation, Vector3 decoyPosition)
    {
        newDecoy = Instantiate(decoy) as GameObject;
        Destroy(newDecoy, destructionTime);
        newDecoy.transform.rotation = decoyRotation;
        newDecoy.GetComponent<DecoyBehaviour>().controller = controller;
        newDecoy.transform.position = new Vector3(decoyPosition.x, decoyPosition.y - 0.8f, decoyPosition.z);

        //newDecoy.animator.SetFloat("Velocity", 1);

        ServerStatsManager.instance.StartDecoyTimer(cooldown);
    }

}
