using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "EditedClip", menuName = "ScriptableObjects/EditedClip")]
public class EditedClip : ScriptableObject {

    public AudioClip Clip;
    [Range(0.1f, 3f)]
    public float Pitch = 1;
    [Range(0f, 1f)]
    public float Volume = 1;
    [Range(5f, 1000f)]
    public float MinDistance = 5f;
    [Range(10f, 1000f)]
    public float MaxDistance = 10f;
    public bool Looping;
    public AudioMixerGroup AudioMixer;

    private AudioSource _source;

    //Used to play a the Clip
    public void PlayClip(AudioSource source) {
        source.pitch = Pitch;
        source.volume = Volume;
        source.loop = Looping;
        source.playOnAwake = false;
        source.outputAudioMixerGroup = AudioMixer;
        source.clip = Clip;
        source.minDistance = MinDistance;
        source.maxDistance = MaxDistance;
        _source = source;
        source.Play();
    }

    public float GetLength() {
        return Clip.length;
    }

    public void Pause() {
        if(_source != null)
            _source.Pause();
    }

    public void UnPause() {
        if (_source != null)
            _source.UnPause();
    }

    public void Stop() {
        if (_source != null)
            _source.Stop();
    }

    public AudioSource GetSource() {
        return _source;
    }
}
