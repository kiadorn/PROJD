using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {


    [SerializeField]
    protected GameObject[] views;
    
    [Header("Volume")]
    [SerializeField]
    protected AudioMixer mixer;
    [SerializeField]
    protected FloatVariable volume;

    private void Start()
    {
        mixer.SetFloat("Master", volume.Value);
    }

    public void SwitchViewTo(GameObject objectToEnable)
    {
        foreach (GameObject g in views)
        {
            g.SetActive(false);
        }
        objectToEnable.SetActive(true);
    }

    public void VolumeChanged(Slider slider)
    {
        volume.Value = slider.value;
        mixer.SetFloat("Master", volume.Value);
    }
}
