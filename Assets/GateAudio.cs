using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAudio : MonoBehaviour {
    
    public static GateAudio instance;
    public GameObject[] gates;

    private void Start () {
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
	
	public void PlayIdle()
    {
        foreach (GameObject gate in gates)
        {
            SoundManager.instance.PlayGateSound(gate);
        }
    }

    public void PlayOpen()
    {
        foreach (GameObject gate in gates)
        {
            SoundManager.instance.PlayGateOpen(gate);
        }
    }
}
