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
            SoundManager.instance.PlayGateSound(gate.transform.gameObject);
            gate.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);
        }
    }

    public void PlayOpen()
    {
        foreach (GameObject gate in gates)
        {
            SoundManager.instance.PlayGateOpen(gate.transform.gameObject);
            gate.GetComponent<Renderer>().material.SetFloat("_Alpha", 1);
        }
    }
}
