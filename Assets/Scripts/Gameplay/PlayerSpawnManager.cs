using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour {

    public static PlayerSpawnManager instance;
    public Transform[] teamWhiteSpawns;
    public Transform[] teamBlackSpawns;
    public Vector3 spawnOffset;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }

    public void Spawn(GameObject player)
    {
        Transform spawn;
        PlayerController controller = player.GetComponent<PlayerController>();
        Transform[] listToUse;
        if (player.GetComponent<PlayerController>().myTeamID == 1)
        {
            listToUse = teamWhiteSpawns;
        } else
        {
            listToUse = teamBlackSpawns;
        }
        spawn = listToUse[Random.Range(0, listToUse.Length)];
        player.GetComponent<ThirdPersonAnimationController>().CancelCharge();
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        controller.mouseLook.ResetRotation(player.transform, controller.cam.transform, spawn.localRotation.eulerAngles.y);
        player.transform.position = spawn.position + spawnOffset;
        controller.isCharging = false;
        controller.beamDistance = 0;
        controller.firstPersonChargeEffect.transform.localScale = Vector3.zero;
        PersonalUI.instance.UpdateShootCharge(0, 1);
        if (controller.isLocalPlayer)
            controller.CmdSendSpawnLocation(player.transform.position);
    }
}
