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
                SoundManager.instance.PlayDecoyUse();
                cooldown = abilityCooldown;
                //GameObject newDecoy = CreateDecoy(transform.rotation, transform.position);
                CmdCreateDecoy(transform.rotation, transform.position);
                PersonalUI.instance.StartDecoyTimer(cooldown);
                
            }

            else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse5)) && cooldown > 0)
            {
                SoundManager.instance.PlayActionUnavailable();
                Debug.Log("On cooldown - [" + cooldown + "s remaining]");
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
        //newDecoy.GetComponent<DecoyBehaviour>().thirdPersonMask.material = GetComponent<MaterialSwap>().firstPersonModel.material;
        //newDecoy.GetComponent<DecoyBehaviour>().thirdPersonModel.material = GetComponent<MaterialSwap>().firstPersonModel.material;
        newDecoy.GetComponent<DecoyBehaviour>().thirdPersonMask.material.SetColor("_Color", GetComponent<PlayerController>().myAsset.MaskColor);
        newDecoy.GetComponent<DecoyBehaviour>().thirdPersonModel.material.SetColor("_Color", GetComponent<PlayerController>().myAsset.BodyColor);
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
        serverDecoy.GetComponent<DecoyBehaviour>().thirdPersonMask.material.SetColor("_Inner_Color", GetComponent<PlayerController>().myAsset.MaskColor);
        serverDecoy.GetComponent<DecoyBehaviour>().thirdPersonModel.material.SetColor("_Inner_Color", GetComponent<PlayerController>().myAsset.BodyColor);
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
