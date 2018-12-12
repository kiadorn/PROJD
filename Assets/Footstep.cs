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
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.position - Vector3.down, Color.red, 2f);
        //if (Physics.Raycast(transform.position, Vector3.down, out _hit, Raydistance, mask))
        //{
        //    Debug.Log("träffar mark");
        //    if (!_footHasStepped)
        //    {
        //        Debug.Log("tar ett steg");
        //        Source.PlayOneShot(Clip);
        //        _footHasStepped = true;
        //    }
        //}
        //else
        //{
        //    Debug.Log("fot i luft");
        //    _footHasStepped = false;
        //}
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
