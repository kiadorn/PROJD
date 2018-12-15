using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour {
    public float Raydistance;
    public AudioSource Source;
    public AudioClip Clip;
    public PlayerController Controller;

    private RaycastHit _hit;
    private int mask;
    private bool _footHasStepped = false;


    private void Start()
    {
        mask = 1 << 8;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && !Controller.Dashing)
        {
            if (!_footHasStepped && !Source.isPlaying )
            {
                Source.clip = Clip;
                Source.Play();
                if (AudioManager.IsSoundPlaying(SoundManager.instance.jumpLanding))
                    Source.Stop();
                
                _footHasStepped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            _footHasStepped = false;
        }
    }

}
