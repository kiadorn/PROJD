using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DecoySpawn : NetworkBehaviour {

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
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown("f") && cooldown <= 0)
            {
                cooldown = abilityCooldown;
                CreateDecoy(transform.rotation, transform.position);
                CmdCreateDecoy(transform.rotation, transform.position);
                PersonalUI.instance.StartDecoyTimer(cooldown);
            }
            if (cooldown > 0)
            {
                cooldown = cooldown - Time.deltaTime;
            }
        }
    }

    [Command]
    private void CmdCreateDecoy(Quaternion decoyRotation, Vector3 decoyPosition)
    {
        RpcCreateDecoy(decoyRotation, decoyPosition);
    }

    
    [ClientRpc]
    private void RpcCreateDecoy(Quaternion decoyRotation, Vector3 decoyPosition)
    {
        if (!isLocalPlayer)
        {
            CreateDecoy(decoyRotation, decoyPosition);
        }       
       
    }

    private void CreateDecoy(Quaternion decoyRotation, Vector3 decoyPosition)
    {
        newDecoy = Instantiate(decoy) as GameObject;
        Destroy(newDecoy, destructionTime);
        newDecoy.transform.rotation = decoyRotation;
        newDecoy.GetComponent<DecoyBehaviour>().controller = controller;
        newDecoy.transform.position = new Vector3(decoyPosition.x, decoyPosition.y - 0.9f, decoyPosition.z);
    }

}
