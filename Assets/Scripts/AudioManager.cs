using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    //Used to be able to use coroutines
    static public AudioManager instance;
    private void Awake() {
        instance = this;
    }

    //For 2D-clips where source is irrelevant
    public static void Play2DClip(EditedClip clip) {
        PlayGeneral(clip);
    }

    //For 3D-clips, uses an object to be used as a parent
    public static void Play3DClip(EditedClip clip, GameObject sourceOfSound) {
        PlayGeneral(clip, sourceOfSound);
    }

    //General method for them
    public static void PlayGeneral(EditedClip clip, GameObject sourceOfSound = null) {
        GameObject soundClip = new GameObject();
        soundClip.AddComponent<AudioSource>();
        if (sourceOfSound != null) {
            soundClip.transform.SetParent(sourceOfSound.transform);
            soundClip.transform.localPosition = new Vector3(0, 0, 0);
            soundClip.GetComponent<AudioSource>().spatialBlend = 1;
        } 
        clip.PlayClip(soundClip.GetComponent<AudioSource>());
        if (!clip.Looping) {
            instance.StartCoroutine(WaitAndDestroy(soundClip.GetComponent<AudioSource>()));
        }
    }

    //Destruction for when clip has finished playing
    public static IEnumerator WaitAndDestroy(AudioSource source) {
        while (source.isPlaying || source.time != 0)
            yield return 0;
        Destroy(source.gameObject);
        yield return 0;
    }



}
