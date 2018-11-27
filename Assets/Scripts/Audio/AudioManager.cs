using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    //Used to be able to use coroutines
    static public AudioManager instance;

    private static List<GameObject> listOfPlayingSounds = new List<GameObject>();

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

    public static void Play3DClipSolo(EditedClip clip, GameObject sourceOfSound)
    {
        PlayGeneral(clip, sourceOfSound);
    }

    //General method for them
    public static void PlayGeneral(EditedClip clip, GameObject sourceOfSound = null) {
        GameObject soundClip = new GameObject();
        soundClip.name = (sourceOfSound ? "3" : "2") + "D EditedClip[" + (clip.Clip ? clip.Clip.name : "nosound") + "]"; //Generates the  name, e.g "3D EditedClip[Jump]"
        soundClip.AddComponent<AudioSource>();
        if (sourceOfSound != null) {
            soundClip.transform.SetParent(sourceOfSound.transform);
            soundClip.transform.localPosition = new Vector3(0, 0, 0);
            soundClip.GetComponent<AudioSource>().spatialBlend = 1;
        }
        listOfPlayingSounds.Add(soundClip);
        clip.PlayClip(soundClip.GetComponent<AudioSource>());
        if (!clip.Looping) {
            instance.StartCoroutine(WaitAndDestroy(soundClip.GetComponent<AudioSource>()));
        }
    }

    //Destruction for when clip has finished playing
    public static IEnumerator WaitAndDestroy(AudioSource source) {
        while (source.isPlaying || source.time != 0)
            yield return 0;
        listOfPlayingSounds.Remove(source.gameObject);
        Destroy(source.gameObject);
        yield return 0;
    }

    public static bool IsSoundPlaying(EditedClip soundClip)
    {

        for (int i = listOfPlayingSounds.Count-1; i > 0; i--)
        {
            //if (listOfPlayingSounds[i] == null)
            //{
            //    listOfPlayingSounds.RemoveAt(i);
            //    continue;
            //}
            if (listOfPlayingSounds[i].GetComponent<AudioSource>().clip == soundClip.Clip)
            {
                print("Success!");
                return true;
            }
        }
        return false;
    }


}
