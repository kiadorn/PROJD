using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DecoySpawn : NetworkBehaviour {

    public GameObject decoy;
    private GameObject newDecoy;
    public PlayerController controller;

    public float abilityCooldown = 8f;

    private float cooldown = 0f;

    public float targetTransparency = 0;
	
   

    void Update () {
        if (isLocalPlayer)
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse5)) && cooldown <= 0)
            {
                cooldown = abilityCooldown;
                //GameObject newDecoy = CreateDecoy(transform.rotation, transform.position);
                CmdCreateDecoy(transform.rotation, transform.position);
                PersonalUI.instance.StartDecoyTimer(cooldown);
            }

            else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse5)) && cooldown > 0)
            {
                //Play local error sound
                Debug.Log("Error");
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
        GameObject newDecoy = CreateDecoy(transform.rotation, transform.position);
        NetworkServer.Spawn(newDecoy);
        RpcCreateDecoy(decoyRotation, decoyPosition, newDecoy.GetComponent<NetworkIdentity>().netId);
    }


    [ClientRpc]
    private void RpcCreateDecoy(Quaternion decoyRotation, Vector3 decoyPosition, NetworkInstanceId netID)
    {
        GameObject serverDecoy = ClientScene.FindLocalObject(netID);
        serverDecoy.transform.rotation = decoyRotation;
        serverDecoy.GetComponent<DecoyBehaviour>().controller = controller;
        serverDecoy.transform.position = new Vector3(decoyPosition.x, decoyPosition.y - 0.9f, decoyPosition.z);
        serverDecoy.GetComponent<DecoyBehaviour>().targetTransparency = targetTransparency;
    }

    private GameObject CreateDecoy(Quaternion decoyRotation, Vector3 decoyPosition)
    {
        newDecoy = Instantiate(decoy) as GameObject;
        //Destroy(newDecoy, destructionTime);
        //newDecoy.transform.rotation = decoyRotation;
        //newDecoy.GetComponent<DecoyBehaviour>().controller = controller;
        //newDecoy.transform.position = new Vector3(decoyPosition.x, decoyPosition.y - 0.9f, decoyPosition.z);
        //newDecoy.GetComponent<DecoyBehaviour>().targetTransparency = targetTransparency;

        return newDecoy;
    }

}
