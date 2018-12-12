using UnityEngine;
using UnityEngine.Networking;

public class MinimapCameraConnector : MonoBehaviour {

    private void Start()
    {
        RoundManager.instance.OnStartGame += ConnectMinimapCameraToPlayer;
    }

    public void ConnectMinimapCameraToPlayer()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                transform.SetParent(player.transform);
            }
        }
    }
}
