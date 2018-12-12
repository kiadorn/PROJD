using UnityEngine;
using UnityEngine.Networking;

public class MinimapCameraConnector : MonoBehaviour {

    private void Start()
    {
        RoundManager.instance.OnStartGame += ConnectMinimapCameraToPlayer;
    }

    private void LateUpdate() {
        AdjustHeight();
    }

    public void ConnectMinimapCameraToPlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                transform.SetParent(player.transform, false);
            }
        }
    }

    private void AdjustHeight() {
        transform.position = new Vector3(transform.position.x , 130, transform.position.z);
    }
}
